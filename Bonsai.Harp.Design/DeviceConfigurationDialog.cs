using Bonsai.Design;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bonsai.Harp.Design
{
    partial class DeviceConfigurationDialog : Form
    {
        readonly Device instance;
        readonly IObservable<HarpMessage> source;
        readonly float bootloaderInfoWidth;
        readonly Size bootloaderMinimumSize;
        readonly Size deviceInfoMinimumSize;
        readonly DeviceConfiguration configuration;
        readonly PortNameConverter portNameConverter;
        IDisposable subscription;

        public DeviceConfigurationDialog(Device device)
        {
            InitializeComponent();
            source = CreateDevice();
            configuration = new DeviceConfiguration();
            portNameConverter = new PortNameConverter();
            portNameComboBox.Text = device.PortName;
            propertyGrid.SelectedObject = configuration;
            bootloaderInfoWidth = tableLayoutPanel.ColumnStyles[1].Width;
            bootloaderMinimumSize = MinimumSize;
            deviceInfoMinimumSize = new Size(bootloaderGroupBox.Left + EdgeMargin, MinimumSize.Height);
            warningTextBox.Select(0, warningTextBox.Find(":") + 1);
            warningTextBox.SelectionFont = new Font(warningTextBox.Font, FontStyle.Bold);
            instance = device;
        }

        private IObservable<HarpMessage> CreateDevice()
        {
            var resetDeviceName = Observable.FromEventPattern(
                handler => resetNameButton.Click += handler,
                handler => resetNameButton.Click -= handler)
                .Select(evt => ShouldResetDeviceName())
                .Where(result => result != DialogResult.Cancel)
                .Select(result => ResetDevice.FromPayload(MessageType.Write, ResetFlags.RestoreName))
                .Publish().RefCount();

            var resetDeviceSettings = Observable.FromEventPattern(
                handler => resetSettingsButton.Click += handler,
                handler => resetSettingsButton.Click -= handler)
                .Select(evt => ShouldResetPersistentRegisters())
                .Where(result => result != DialogResult.Cancel)
                .SelectMany(result => ResetRegisters(true))
                .Publish().RefCount();

            var device = Observable.Defer(() => new Device
            {
                PortName = instance.PortName,
                Heartbeat = EnableFlag.Disabled,
                IgnoreErrors = true
            }.Generate(resetDeviceName.Merge(resetDeviceSettings)));

            return device
                .Where(MessageType.Read).Do(ReadRegister)
                .Throttle(TimeSpan.FromSeconds(0.2))
                .ObserveOn(propertyGrid)
                .Do(message => ValidateDevice())
                .DelaySubscription(TimeSpan.FromSeconds(0.2))
                .TakeUntil(resetDeviceName.Merge(resetDeviceSettings)
                    .Where(ResetDevice.Address)
                    .Delay(TimeSpan.FromSeconds(1)))
                .Do(x => { }, () => BeginInvoke((Action)WaitForReset))
                .Catch<HarpMessage, IOException>(ConnectionErrorHandler);
        }

        private void ValidateDevice()
        {
            propertyGrid.Refresh();
            SetConnectionStatus(ConnectionStatus.Ready);
        }

        private void OpenDevice()
        {
            SetConnectionStatus(ConnectionStatus.Open);
            subscription = source.Subscribe();
        }

        private void CloseDevice()
        {
            if (subscription != null)
            {
                subscription.Dispose();
                subscription = null;
            }
        }

        private void WaitForReset()
        {
            CloseDevice();
            SetConnectionStatus(ConnectionStatus.Reset);
            using (var resetDialog = new DeviceResetDialog())
            {
                resetDialog.ShowDialog(this);
            }
            OpenDevice();
        }

        private IObservable<HarpMessage> ConnectionErrorHandler(IOException ex)
        {
            SetConnectionStatus(ConnectionStatus.Closed);
            return Observable.Empty<HarpMessage>();
        }

        private DialogResult ShouldResetDeviceName()
        {
            return MessageBox.Show(this,
                Properties.Resources.ResetDeviceName_Question,
                Text, MessageBoxButtons.OKCancel);
        }

        private DialogResult ShouldResetPersistentRegisters()
        {
            return MessageBox.Show(this,
                Properties.Resources.ResetPersistentRegisters_Question,
                Text, MessageBoxButtons.OKCancel);
        }

        IEnumerable<HarpMessage> ResetRegisters(bool resetDefault)
        {
            var resetMode = resetDefault ? ResetFlags.RestoreDefault : ResetFlags.RestoreEeprom;
            yield return ResetDevice.FromPayload(MessageType.Write, resetMode);
        }

        private void ReadRegister(HarpMessage message)
        {
            switch (message.Address)
            {
                case WhoAmI.Address:
                    configuration.WhoAmI = WhoAmI.GetPayload(message);
                    break;
                case HardwareVersionHigh.Address:
                    configuration.HardwareVersionHigh = HardwareVersionHigh.GetPayload(message);
                    break;
                case HardwareVersionLow.Address:
                    configuration.HardwareVersionLow = HardwareVersionLow.GetPayload(message);
                    break;
                case FirmwareVersionHigh.Address:
                    configuration.FirmwareVersionHigh = FirmwareVersionHigh.GetPayload(message);
                    break;
                case FirmwareVersionLow.Address:
                    configuration.FirmwareVersionLow = FirmwareVersionLow.GetPayload(message);
                    break;
                case CoreVersionHigh.Address:
                    configuration.CoreVersionHigh = CoreVersionHigh.GetPayload(message);
                    break;
                case CoreVersionLow.Address:
                    configuration.CoreVersionLow = CoreVersionLow.GetPayload(message);
                    break;
                case AssemblyVersion.Address:
                    configuration.AssemblyVersion = AssemblyVersion.GetPayload(message);
                    break;
                case TimestampSeconds.Address:
                    configuration.Timestamp = TimestampSeconds.GetPayload(message);
                    break;
                case DeviceName.Address:
                    var deviceName = nameof(Device);
                    if (!message.Error) deviceName = DeviceName.GetPayload(message);
                    configuration.DeviceName = deviceName;
                    break;
                case SerialNumber.Address:
                    configuration.SerialNumber = SerialNumber.GetPayload(message);
                    break;
                default:
                    break;
            }
        }

        void SetConnectionStatus(ConnectionStatus status)
        {
            switch (status)
            {
                case ConnectionStatus.Open:
                    connectionStatusLabel.Text = "Connecting to device...";
                    break;
                case ConnectionStatus.Ready:
                    connectionStatusLabel.Text = $"Ready ({instance.PortName})";
                    break;
                case ConnectionStatus.Reset:
                    connectionStatusLabel.Text = "Resetting device...";
                    break;
                case ConnectionStatus.Closed:
                    connectionStatusLabel.Text = $"No device connected ({instance.PortName})";
                    break;
                default:
                    break;
            }
        }

        void SelectFirmware(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            updateFirmwareButton.Enabled = FirmwareMetadata.TryParse(fileName, out FirmwareMetadata metadata);
            firmwarePropertyGrid.SelectedObject = metadata;
        }

        void UpdateFirmware(string path)
        {
            var deviceFirmware = DeviceFirmware.FromFile(path);
            var forceUpdate = ModifierKeys == (Keys.Shift | Keys.Control | Keys.Alt);
            if (!deviceFirmware.Metadata.Supports(configuration.DeviceName, configuration.HardwareVersion) &&
                !forceUpdate)
            {
                MessageBox.Show(this,
                    Properties.Resources.UpdateDeviceFirmware_NotSupported,
                    Properties.Resources.UpdateDeviceFirmware_Caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else if (MessageBox.Show(this,
                Properties.Resources.UpdateDeviceFirmware_Question,
                Properties.Resources.UpdateDeviceFirmware_Caption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                CloseDevice();
                SetConnectionStatus(ConnectionStatus.Reset);
                using (var firmwareDialog = new DeviceOperationDialog(
                    Properties.Resources.UpdateDeviceFirmware_Label,
                    Properties.Resources.UpdateDeviceFirmware_Caption,
                    progress => Bootloader.UpdateFirmwareAsync(
                        instance.PortName,
                        deviceFirmware,
                        forceUpdate,
                        progress)))
                {
                    firmwareDialog.ShowDialog(this);
                }
                WaitForReset();
            }
        }

        bool UpdateDeviceName(string name)
        {
            if (MessageBox.Show(this,
                Properties.Resources.UpdateDeviceName_Question, Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                CloseDevice();
                SetConnectionStatus(ConnectionStatus.Reset);
                using (var firmwareDialog = new DeviceOperationDialog(
                    Properties.Resources.UpdateDeviceName_Label,
                    Properties.Resources.UpdateDeviceName_Caption,
                    progress => UpdateDeviceNameAsync(instance.PortName, name, progress)))
                {
                    firmwareDialog.ShowDialog(this);
                }
                WaitForReset();
                return true;
            }

            return false;
        }

        static async Task UpdateDeviceNameAsync(string portName, string name, IProgress<int> progress)
        {
            using (var device = new AsyncDevice(portName))
            {
                progress?.Report(40);
                await device.WriteDeviceNameAsync(name);
            }
            await Observable.Timer(TimeSpan.FromMilliseconds(200));
            progress?.Report(100);
        }

        int EdgeMargin => Width - tableLayoutPanel.Right - tableLayoutPanel.Margin.Right;

        void ToggleBootloader()
        {
            var bootloaderStyle = tableLayoutPanel.ColumnStyles[1];
            SuspendLayout();
            if (bootloaderStyle.Width > 0)
            {
                var collapseWidth = bootloaderGroupBox.Left + EdgeMargin;
                MinimumSize = deviceInfoMinimumSize;
                Width = collapseWidth;
                bootloaderStyle.Width = 0;
            }
            else
            {
                var remainderWidth = 100 - bootloaderInfoWidth;
                var fullWidth = (int)(100 * (deviceInfoGroupBox.Right + tableLayoutPanel.Margin.Right + 1) / remainderWidth);
                var expandWidth = fullWidth + EdgeMargin + tableLayoutPanel.Margin.Right;
                MinimumSize = bootloaderMinimumSize;
                Width = expandWidth;
                bootloaderStyle.Width = bootloaderInfoWidth;
            }
            ResumeLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            ToggleBootloader();
            OpenDevice();
            base.OnLoad(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            CloseDevice();
            base.OnFormClosed(e);
        }

        enum ConnectionStatus
        {
            Open,
            Ready,
            Reset,
            Closed
        }

        private void selectFirmwareButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SelectFirmware(openFileDialog.FileName);
            }
        }

        private void updateFirmwareButton_Click(object sender, EventArgs e)
        {
            UpdateFirmware(openFileDialog.FileName);
        }

        private void bootloaderButton_Click(object sender, EventArgs e)
        {
            ToggleBootloader();
        }

        private void propertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == nameof(DeviceConfiguration.DeviceName))
            {
                var name = (string)e.ChangedItem.Value;
                if (string.IsNullOrEmpty(name) || !UpdateDeviceName(name))
                {
                    e.ChangedItem.PropertyDescriptor.SetValue(configuration, e.OldValue);
                    propertyGrid.Refresh();
                }
            }
        }

        private void portNameComboBox_DropDown(object sender, EventArgs e)
        {
            var portNames = portNameConverter.GetStandardValues();
            portNameBindingSource.DataSource = portNames.Count > 0 ? portNames : null;
        }

        private void portNameComboBox_Validated(object sender, EventArgs e)
        {
            if (instance != null)
            {
                instance.PortName = portNameComboBox.Text;
                CloseDevice();
                OpenDevice();
            }
        }

        private void portNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            propertyGrid.Select();
        }
    }
}

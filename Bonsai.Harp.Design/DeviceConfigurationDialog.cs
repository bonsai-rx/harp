using Bonsai.Design;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
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
        IDisposable subscription;

        public DeviceConfigurationDialog(Device device)
        {
            InitializeComponent();
            instance = device;
            source = CreateDevice();
            configuration = new DeviceConfiguration();
            propertyGrid.SelectedObject = configuration;
            bootloaderInfoWidth = tableLayoutPanel.ColumnStyles[1].Width;
            bootloaderMinimumSize = MinimumSize;
            deviceInfoMinimumSize = new Size(bootloaderGroupBox.Left + EdgeMargin, MinimumSize.Height);
            warningTextBox.Select(0, warningTextBox.Find(":") + 1);
            warningTextBox.SelectionFont = new Font(warningTextBox.Font, FontStyle.Bold);
        }

        private IObservable<HarpMessage> CreateDevice()
        {
            var device = new Device
            {
                PortName = instance.PortName,
                Heartbeat = EnableType.Disable,
                IgnoreErrors = true
            };

            var resetDeviceName = Observable.FromEventPattern(
                handler => resetNameButton.Click += handler,
                handler => resetNameButton.Click -= handler)
                .Select(evt => ShouldResetDeviceName())
                .Where(result => result != DialogResult.Cancel)
                .Select(result => HarpCommand.ResetDevice(ResetMode.RestoreName))
                .Publish().RefCount();

            var resetDeviceSettings = Observable.FromEventPattern(
                handler => resetSettingsButton.Click += handler,
                handler => resetSettingsButton.Click -= handler)
                .Select(evt => ShouldResetPersistentRegisters())
                .Where(result => result != DialogResult.Cancel)
                .SelectMany(result => ResetRegisters(true))
                .Publish().RefCount();

            return device.Generate(resetDeviceName.Merge(resetDeviceSettings))
                .Where(MessageType.Read).Do(ReadRegister)
                .Throttle(TimeSpan.FromSeconds(0.2))
                .ObserveOn(propertyGrid)
                .Do(message => ValidateDevice())
                .DelaySubscription(TimeSpan.FromSeconds(0.2))
                .TakeUntil(resetDeviceName.Merge(resetDeviceSettings)
                    .Where(DeviceRegisters.ResetDevice)
                    .Delay(TimeSpan.FromSeconds(1)))
                .Do(x => { }, () => BeginInvoke((Action)ResetDevice));
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

        private void ResetDevice()
        {
            CloseDevice();
            SetConnectionStatus(ConnectionStatus.Reset);
            using (var resetDialog = new DeviceResetDialog())
            {
                resetDialog.ShowDialog(this);
            }
            OpenDevice();
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
            var resetMode = resetDefault ? ResetMode.RestoreDefault : ResetMode.RestoreEeprom;
            yield return HarpCommand.ResetDevice(resetMode);
        }

        private void ReadRegister(HarpMessage message)
        {
            switch (message.Address)
            {
                case DeviceRegisters.WhoAmI:
                    configuration.WhoAmI = message.GetPayloadUInt16();
                    break;
                case DeviceRegisters.HardwareVersionHigh:
                    configuration.HardwareVersionHigh = message.GetPayloadByte();
                    break;
                case DeviceRegisters.HardwareVersionLow:
                    configuration.HardwareVersionLow = message.GetPayloadByte();
                    break;
                case DeviceRegisters.FirmwareVersionHigh:
                    configuration.FirmwareVersionHigh = message.GetPayloadByte();
                    break;
                case DeviceRegisters.FirmwareVersionLow:
                    configuration.FirmwareVersionLow = message.GetPayloadByte();
                    break;
                case DeviceRegisters.CoreVersionHigh:
                    configuration.CoreVersionHigh = message.GetPayloadByte();
                    break;
                case DeviceRegisters.CoreVersionLow:
                    configuration.CoreVersionLow = message.GetPayloadByte();
                    break;
                case DeviceRegisters.AssemblyVersion:
                    configuration.AssemblyVersion = message.GetPayloadByte();
                    break;
                case DeviceRegisters.TimestampSecond:
                    configuration.Timestamp = message.GetPayloadUInt32();
                    break;
                case DeviceRegisters.DeviceName:
                    var deviceName = nameof(Device);
                    if (!message.Error)
                    {
                        var namePayload = message.GetPayload();
                        deviceName = Encoding.ASCII.GetString(namePayload.Array, namePayload.Offset, namePayload.Count).Trim('\0');
                    }
                    configuration.DeviceName = deviceName;
                    break;
                case DeviceRegisters.SerialNumber:
                    configuration.SerialNumber = message.GetPayloadUInt16();
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
                    connectionStatusLabel.Text = $"Ready ({instance.PortName}: {configuration.Id})";
                    break;
                case ConnectionStatus.Reset:
                    connectionStatusLabel.Text = "Resetting device...";
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
            if (MessageBox.Show(this,
                Properties.Resources.UpdateDeviceFirmware_Question, Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                CloseDevice();
                SetConnectionStatus(ConnectionStatus.Reset);
                var deviceFirmware = DeviceFirmware.FromFile(path);
                using (var firmwareDialog = new DeviceOperationDialog(
                    Properties.Resources.UpdateDeviceFirmware_Label,
                    Properties.Resources.UpdateDeviceFirmware_Caption,
                    progress => Bootloader.UpdateFirmwareAsync(instance.PortName, deviceFirmware, progress)))
                {
                    firmwareDialog.ShowDialog(this);
                }
                ResetDevice();
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
                ResetDevice();
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
            Reset
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
    }
}

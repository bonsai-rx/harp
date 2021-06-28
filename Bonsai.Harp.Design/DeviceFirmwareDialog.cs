using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bonsai.Harp.Design
{
    public partial class DeviceFirmwareDialog : Form
    {
        readonly Func<Task> updateFirmwareAsync;

        public DeviceFirmwareDialog(string portName, DeviceFirmware firmware)
        {
            if (firmware == null)
            {
                throw new ArgumentNullException(nameof(firmware));
            }

            InitializeComponent();
            var progress = new Progress<int>(ReportProgress);
            updateFirmwareAsync = () => Bootloader.UpdateFirmwareAsync(portName, firmware, progress);
            Text = string.Format(Text, firmware.Metadata.DeviceName);
            updateLabel.Text = string.Format(updateLabel.Text, firmware.Metadata.DeviceName);
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            var size = ClientSize;
            ClientSize = new Size((int)(size.Width * factor.Width), (int)(size.Height * factor.Height));
        }

        protected override void OnLoad(EventArgs e)
        {
            updateFirmwareAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    MessageBox.Show(this, task.Exception.InnerException.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else DialogResult = DialogResult.OK;
                Close();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            base.OnLoad(e);
        }

        private void ReportProgress(int value)
        {
            progressBar.Value = value;
        }
    }
}

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bonsai.Harp.Design
{
    internal partial class DeviceOperationDialog : Form
    {
        readonly Func<Task> operationAsync;

        public DeviceOperationDialog(string text, string caption, Func<IProgress<int>, Task> operationAsyncFactory)
        {
            if (operationAsyncFactory == null)
            {
                throw new ArgumentNullException(nameof(operationAsyncFactory));
            }

            InitializeComponent();
            var progress = new Progress<int>(ReportProgress);
            operationAsync = () => operationAsyncFactory(progress);
            updateLabel.Text = text;
            Text = caption;
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            var size = ClientSize;
            ClientSize = new Size((int)(size.Width * factor.Width), (int)(size.Height * factor.Height));
        }

        protected override void OnLoad(EventArgs e)
        {
            operationAsync().ContinueWith(task =>
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

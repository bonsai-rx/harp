using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bonsai.Harp.Design
{
    partial class DeviceResetDialog : Form
    {
        public DeviceResetDialog()
        {
            InitializeComponent();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            var size = ClientSize;
            ClientSize = new Size((int)(size.Width * factor.Width), (int)(size.Height * factor.Height));
        }

        protected override void OnLoad(EventArgs e)
        {
            resetTimer.Start();
            base.OnLoad(e);
        }

        private void resetTimer_Tick(object sender, EventArgs e)
        {
            progressBar.Increment(resetTimer.Interval);
            if (progressBar.Value >= progressBar.Maximum)
            {
                Close();
            }
        }
    }
}

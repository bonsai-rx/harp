using System;
using System.Windows.Forms;
using ZedGraph;
using System.Globalization;

namespace Bonsai.Harp.Visualizers
{
    partial class TimelineGraphView : UserControl
    {
        readonly ToolStripEditableLabel timeSpanEditableLabel;

        public TimelineGraphView()
        {
            InitializeComponent();
            timeSpanEditableLabel = new ToolStripEditableLabel(timeSpanValueLabel, OnTimeSpanEdit);
            Graph.GraphPane.AxisChangeEvent += GraphPane_AxisChangeEvent;
            components.Add(timeSpanEditableLabel);
        }

        protected StatusStrip StatusStrip
        {
            get { return statusStrip; }
        }

        public TimelineGraph Graph
        {
            get { return graph; }
        }

        public double TimeSpan { get; set; }

        public bool CanEditTimeSpan
        {
            get { return timeSpanEditableLabel.Enabled; }
            set { timeSpanEditableLabel.Enabled = value; }
        }

        public event EventHandler AxisChanged;

        protected virtual void OnAxisChanged(EventArgs e)
        {
            AxisChanged?.Invoke(this, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private bool graph_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            var pane = graph.MasterPane.FindChartRect(e.Location);
            if (pane != null)
            {
                pane.ReverseTransform(e.Location, out double x, out double y);
                cursorStatusLabel.Text = string.Format("Cursor: ({0:F0}, {1:G5})", x, y);
            }
            return false;
        }

        private void graph_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                statusStrip.Visible = !statusStrip.Visible;
            }
        }

        private void GraphPane_AxisChangeEvent(GraphPane pane)
        {
            var timeSpan = TimeSpan;
            timeSpanValueLabel.Text = timeSpan.ToString(CultureInfo.InvariantCulture);
            OnAxisChanged(EventArgs.Empty);
        }

        private void OnTimeSpanEdit(string text)
        {
            if (int.TryParse(text, out int timeSpan))
            {
                TimeSpan = timeSpan;
            }
        }
    }
}

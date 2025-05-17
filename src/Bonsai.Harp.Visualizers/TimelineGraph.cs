using System.Drawing;
using System.Windows.Forms;
using Bonsai.Design.Visualizers;
using ZedGraph;

namespace Bonsai.Harp.Visualizers
{
    internal class TimelineGraph : GraphControl
    {
        bool autoScaleX;
        bool autoScaleY;

        public TimelineGraph()
        {
            autoScaleX = true;
            autoScaleY = true;
            IsShowContextMenu = false;
            ZoomEvent += RollingGraph_ZoomEvent;
        }

        public double XMin
        {
            get { return GraphPane.XAxis.Scale.Min; }
            set
            {
                GraphPane.XAxis.Scale.Min = value;
                GraphPane.AxisChange();
                Invalidate();
            }
        }

        public double XMax
        {
            get { return GraphPane.XAxis.Scale.Max; }
            set
            {
                GraphPane.XAxis.Scale.Max = value;
                GraphPane.AxisChange();
                Invalidate();
            }
        }

        public double YMin
        {
            get { return GraphPane.YAxis.Scale.Min; }
            set
            {
                GraphPane.YAxis.Scale.Min = value;
                GraphPane.AxisChange();
                Invalidate();
            }
        }

        public double YMax
        {
            get { return GraphPane.YAxis.Scale.Max; }
            set
            {
                GraphPane.YAxis.Scale.Max = value;
                GraphPane.AxisChange();
                Invalidate();
            }
        }

        public bool AutoScaleX
        {
            get { return autoScaleX; }
            set
            {
                var changed = autoScaleX != value;
                autoScaleX = value;
                if (changed)
                {
                    GraphPane.XAxis.Scale.MaxAuto = autoScaleX;
                    GraphPane.XAxis.Scale.MinAuto = autoScaleX;
                    if (autoScaleX) Invalidate();
                }
            }
        }

        public bool AutoScaleY
        {
            get { return autoScaleY; }
            set
            {
                var changed = autoScaleY != value;
                autoScaleY = value;
                if (changed)
                {
                    GraphPane.YAxis.Scale.MaxAuto = autoScaleY;
                    GraphPane.YAxis.Scale.MinAuto = autoScaleY;
                    if (autoScaleY) Invalidate();
                }
            }
        }

        internal CurveItem CreateSeries(string label, IPointListEdit points, Color color)
        {
            var curve = new LineItem(label, points, color, SymbolType.Circle, lineWidth: 0);
            curve.Line.IsAntiAlias = false;
            curve.Line.IsOptimizedDraw = true;
            curve.Label.IsVisible = !string.IsNullOrEmpty(label);
            curve.Symbol.Fill.Type = FillType.Solid;
            curve.Symbol.IsAntiAlias = true;
            return curve;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
            {
                DoPrint();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                SaveAs();
            }

            if (e.KeyCode == Keys.Back)
            {
                ZoomOut(GraphPane);
            }

            base.OnKeyDown(e);
        }

        private void RollingGraph_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            MasterPane.AxisChange();
        }
    }
}

using System;
using ZedGraph;

namespace Bonsai.Harp.Visualizers
{
    static class GraphHelper
    {
        internal static void SetAxisLabel(Axis axis, string label)
        {
            axis.Title.Text = label;
            axis.Title.IsVisible = !string.IsNullOrEmpty(label);
        }

        internal static void FormatTimeAxis(Axis axis)
        {
            axis.Type = AxisType.Linear;
            axis.Scale.MaxAuto = false;
            axis.Scale.MinAuto = false;
        }
    }
}

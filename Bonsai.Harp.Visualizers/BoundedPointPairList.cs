using System;
using ZedGraph;

namespace Bonsai.Harp.Visualizers
{
    internal class BoundedPointPairList : IPointListEdit
    {
        double minValue = double.MinValue;
        double maxValue = double.MaxValue;
        readonly QueueList<PointPair> points = new();

        public BoundedPointPairList()
        {
        }

        public BoundedPointPairList(IPointList points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Add(points[i]);
            }
        }

        public void SetBounds(double min, double max)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException(nameof(max));
            }

            minValue = min;
            maxValue = max;
            while (points.Count > 0 && points[0].X < min)
            {
                points.TryDequeue(out _);
            }

            while (points.Count > 0 && points[points.Count - 1].X > max)
            {
                points.TryDequeueLast(out _);
            }
        }

        public PointPair this[int index]
        {
            get => points[index];
            set => points[index] = value;
        }

        PointPair IPointList.this[int index] => this[index];

        public int Count => points.Count;

        public void Add(PointPair point)
        {
            if (point.X >= minValue && point.X <= maxValue)
            {
                points.Enqueue(point);
            }
        }

        public void Add(double x, double y)
        {
            if (x >= minValue && x <= maxValue)
            {
                points.Enqueue(new PointPair(x, y));
            }
        }

        public void Clear()
        {
            points.Clear();
        }

        public object Clone()
        {
            return new BoundedPointPairList(this);
        }

        public void RemoveAt(int index)
        {
            points.RemoveAt(index);
        }
    }
}

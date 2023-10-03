using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Forms;
using Bonsai.Design;
using Bonsai.Design.Visualizers;
using Bonsai.Expressions;

namespace Bonsai.Harp.Visualizers
{
    /// <summary>
    /// Provides a type visualizer to display a sequence of Harp messages as a synchronized rolling graph.
    /// </summary>
    public class TimelineGraphVisualizer : DialogTypeVisualizer
    {
        const int TargetInterval = 1000 / 50;
        TimelineGraphBuilder.VisualizerController controller;
        TimelineGraphView view;
        Timer timer;

        /// <summary>
        /// Gets or sets the maximum time range, in seconds, displayed at any one moment in the graph.
        /// </summary>
        public double TimeSpan { get; set; }

        static string GetRegisterInfo(IObservable<HarpMessage> register, out int address)
        {
            switch (register)
            {
                case IGroupedObservable<int, HarpMessage> addressRegister:
                    address = addressRegister.Key;
                    return address.ToString();
                case IGroupedObservable<Type, HarpMessage> typedRegister:
                    var addressField = typedRegister.Key.GetField(nameof(WhoAmI.Address), BindingFlags.Static | BindingFlags.Public);
                    address = (int)addressField.GetValue(null);
                    return typedRegister.Key.Name;
                default:
                    throw new ArgumentException("Unsupported register type.", nameof(register));
            }
        }

        /// <inheritdoc/>
        public override void Load(IServiceProvider provider)
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var timelineBuilder = (TimelineGraphBuilder)ExpressionBuilder.GetVisualizerElement(context.Source).Builder;
            controller = timelineBuilder.Controller;

            timer = new Timer();
            timer.Interval = TargetInterval;
            var timerTick = Observable.FromEventPattern<EventHandler, EventArgs>(
                handler => timer.Tick += handler,
                handler => timer.Tick -= handler);
            timer.Start();

            view = new TimelineGraphView();
            view.Dock = DockStyle.Fill;
            view.Graph.AutoScaleX = false;
            view.TimeSpan = controller.TimeSpan.GetValueOrDefault(TimeSpan);
            view.CanEditTimeSpan = controller.TimeSpan.HasValue;
            GraphHelper.FormatTimeAxis(view.Graph.GraphPane.XAxis);
            GraphHelper.SetAxisLabel(view.Graph.GraphPane.XAxis, "Time");
            GraphHelper.SetAxisLabel(view.Graph.GraphPane.YAxis, "Register");

            var currentTime = 0.0;
            var absoluteMinTime = double.MaxValue;
            CompositeDisposable subscriptions = new();
            view.HandleCreated += delegate
            {
                subscriptions.Add(controller.Registers.ObserveOn(view).Subscribe(register =>
                {
                    var label = GetRegisterInfo(register, out int address);
                    var color = GraphControl.GetColor(address);
                    var points = new BoundedPointPairList();
                    subscriptions.Add(register
                    .Select(message => message.GetTimestamp())
                    .Buffer(() => timerTick)
                    .Subscribe(buffer =>
                    {
                        if (buffer.Count == 0) return;
                        foreach (var timestamp in buffer)
                        {
                            absoluteMinTime = Math.Min(absoluteMinTime, timestamp);
                            currentTime = Math.Max(currentTime, timestamp);
                            points.Add(timestamp, address);
                        }

                        if (view.TimeSpan > 0)
                        {
                            var relativeMinTime = currentTime - view.TimeSpan;
                            foreach (var series in view.Graph.GraphPane.CurveList)
                            {
                                ((BoundedPointPairList)series.Points).SetBounds(
                                    relativeMinTime,
                                    double.MaxValue);
                            }
                            view.Graph.XMin = Math.Max(absoluteMinTime, relativeMinTime);
                        }
                        else view.Graph.XMin = absoluteMinTime;
                        view.Graph.XMax = currentTime;
                        view.Graph.Invalidate();
                    }));
                    var series = view.Graph.CreateSeries(label, points, color);
                    view.Graph.GraphPane.CurveList.Add(series);
                    view.Graph.Invalidate();
                }));
            };

            view.HandleDestroyed += delegate
            {
                subscriptions.Dispose();
                TimeSpan = view.TimeSpan;
            };

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            visualizerService?.AddControl(view);
        }

        /// <inheritdoc/>
        public override void Show(object value)
        {
        }

        /// <inheritdoc/>
        public override IObservable<object> Visualize(IObservable<IObservable<object>> source, IServiceProvider provider)
        {
            return Observable.Empty<object>();
        }

        /// <inheritdoc/>
        public override void Unload()
        {
            view?.Dispose();
            timer?.Dispose();
            view = null;
            timer = null;
            controller = null;
        }
    }
}

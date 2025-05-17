using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which produces an observable sequence of Harp messages from a previously recorded data file.
    /// </summary>
    [Description("Produces a sequence of Harp messages from a previously recorded data file.")]
    public class FileDevice : Source<HarpMessage>
    {
        /// <summary>
        /// Gets or sets the path to the binary file containing Harp messages to playback.
        /// </summary>
        [Description("The path to the binary file containing Harp messages.")]
        [Editor("Bonsai.Design.OpenFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore any device error messages included in the binary file.
        /// </summary>
        [Description("Indicates whether device errors should be ignored.")]
        public bool IgnoreErrors { get; set; }

        /// <summary>
        /// Gets or sets the optional rate multiplier to either slowdown or speedup the playback. If
        /// no rate is specified, playback will be done as fast as possible.
        /// </summary>
        [Description("The optional rate multiplier to either slowdown or speedup the playback. If no rate is specified, playback will be done as fast as possible.")]
        public double? PlaybackRate { get; set; } = 1;

        /// <summary>
        /// Opens the specified file name and returns the observable sequence of Harp messages
        /// stored in the binary file.
        /// </summary>
        /// <returns>The observable sequence of Harp messages stored in the binary file.</returns>
        public override IObservable<HarpMessage> Generate()
        {
            const int ReadBufferSize = 4096;
            var fileName = FileName;
            var ignoreErrors = IgnoreErrors;
            return Observable.Create<HarpMessage>((observer, cancellationToken) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    using var stream = new FileStream(fileName, FileMode.Open);
                    using var waitSignal = new ManualResetEvent(false);
                    double timestampOffset = 0;
                    var stopwatch = new Stopwatch();

                    var harpObserver = Observer.Create<HarpMessage>(
                        value =>
                        {
                            var playbackRate = PlaybackRate;
                            if (playbackRate.HasValue && value.TryGetTimestamp(out double timestamp))
                            {
                                timestamp *= 1000.0 / playbackRate.Value; //ms
                                if (!stopwatch.IsRunning ||
                                    value.MessageType == MessageType.Write &&
                                    value.Address == TimestampSeconds.Address &&
                                    value.PayloadType == (PayloadType.Timestamp | TimestampSeconds.RegisterType))
                                {
                                    stopwatch.Restart();
                                    timestampOffset = timestamp;
                                }

                                var waitInterval = timestamp - timestampOffset - stopwatch.ElapsedMilliseconds;
                                if (waitInterval > 0)
                                {
                                    waitSignal.WaitOne((int)waitInterval);
                                }
                            }

                            observer.OnNext(value);
                        },
                        observer.OnError,
                        observer.OnCompleted);
                    var transport = new StreamTransport(harpObserver);
                    transport.IgnoreErrors = ignoreErrors;

                    long bytesToRead;
                    while (!cancellationToken.IsCancellationRequested &&
                           (bytesToRead = Math.Min(ReadBufferSize, stream.Length - stream.Position)) > 0)
                    {
                        transport.ReceiveData(stream, ReadBufferSize, (int)bytesToRead);
                    }
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            });
        }
    }
}

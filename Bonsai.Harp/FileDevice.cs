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
    [Description("Produces a sequence of Harp messages from a previously recorded data file.")]
    public class FileDevice : Source<HarpMessage>
    {
        readonly IObservable<HarpMessage> source;
        readonly object captureLock = new object();
        const int ReadBufferSize = 4096;

        public FileDevice()
        {
            PlaybackRate = 1;
            source = Observable.Create<HarpMessage>((observer, cancellationToken) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    lock (captureLock)
                    {
                        using (var stream = new FileStream(FileName, FileMode.Open))
                        using (var waitSignal = new ManualResetEvent(false))
                        {
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
                                            value.Address == Registers.TimestampSecond &&
                                            value.PayloadType == (PayloadType.Timestamp | Registers.TimestampSecondPayload))
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
                            transport.IgnoreErrors = IgnoreErrors;

                            int bytesToRead;
                            while (!cancellationToken.IsCancellationRequested &&
                                   (bytesToRead = Math.Min(ReadBufferSize, (int)(stream.Length - stream.Position))) > 0)
                            {
                                transport.ReceiveData(stream, ReadBufferSize, bytesToRead);
                            }
                        }
                    }
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            })
            .PublishReconnectable()
            .RefCount();
        }

        [Description("The path to the binary file containing harp messages.")]
        [Editor("Bonsai.Design.OpenFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string FileName { get; set; }

        [Description("Indicates whether device errors should be ignored.")]
        public bool IgnoreErrors { get; set; }

        [Description("The optional rate multiplier to either slowdown or speedup the playback. If no rate is specified, playback will be done as fast as possible.")]
        public double? PlaybackRate { get; set; }

        public override IObservable<HarpMessage> Generate()
        {
            return source;
        }
    }
}

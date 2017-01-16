using Bonsai;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Bonsai.Harp
{
    public class Device : Source<HarpDataFrame>
    {
        [TypeConverter(typeof(PortNameConverter))]
        public string PortName { get; set; }

        public override IObservable<HarpDataFrame> Generate()
        {
            return Observable.Create<HarpDataFrame>(observer =>
            {
                var transport = new SerialTransport(PortName, observer);
                transport.Open();
                return transport;
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<HarpDataFrame> source)
        {
            return Observable.Create<HarpDataFrame>(observer =>
            {
                var transport = new SerialTransport(PortName, observer);
                transport.Open();

                var sourceDisposable = new SingleAssignmentDisposable();
                sourceDisposable.Disposable = source.Do(
                    input => transport.Write(input),
                    ex => observer.OnError(ex),
                    () => observer.OnCompleted()).Subscribe();

                return new CompositeDisposable(
                    sourceDisposable,
                    transport);
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    public class ReceiveMessage : Source<HarpDataFrame>
    {
        IObservable<HarpDataFrame> source;

        public ReceiveMessage()
        {
            source = Observable.Create<HarpDataFrame>(observer =>
            {
                var transport = new SerialTransport(PortName, observer);
                transport.Open();
                return transport;
            })
            .PublishReconnectable()
            .RefCount();
        }

        [TypeConverter(typeof(PortNameConverter))]
        public string PortName { get; set; }

        public override IObservable<HarpDataFrame> Generate()
        {
            return source;
        }
    }
}

using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that groups a sequence of Harp messages by register address.
    /// </summary>
    [Description("Groups a sequence of Harp messages by register address.")]
    public class GroupByRegister : Combinator<HarpMessage, IGroupedObservable<int, HarpMessage>>
    {
        /// <summary>
        /// Groups an observable sequence of Harp messages by register address.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of observable groups, each of which corresponds to a unique
        /// register address.
        /// </returns>
        public override IObservable<IGroupedObservable<int, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(message => message.Address);
        }
    }
}

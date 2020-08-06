using System;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides a set of static extension methods to aid in writing queries over observable Harp message sequences.
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Filters the elements of an observable sequence of Harp messages based
        /// on their address.
        /// </summary>
        /// <param name="source">The observable sequence whose messages to filter.</param>
        /// <param name="address">The address to test for a match.</param>
        /// <returns>
        /// An observable sequence that contains messages from the input sequence that
        /// match the specified <paramref name="address"/>.
        /// </returns>
        public static IObservable<HarpMessage> Where(this IObservable<HarpMessage> source, int address)
        {
            return source.Where(message => message.IsMatch(address) && !message.Error);
        }

        /// <summary>
        /// Filters the elements of an observable sequence of Harp messages based
        /// on their address.
        /// </summary>
        /// <param name="source">The observable sequence whose messages to filter.</param>
        /// <param name="address">The address to test for a match.</param>
        /// <param name="allowErrors"><c>true</c> to allow error messages in the filter; otherwise, <c>false</c>.</param>
        /// <returns>
        /// An observable sequence that contains messages from the input sequence that
        /// match the specified <paramref name="address"/>.
        /// </returns>
        public static IObservable<HarpMessage> Where(this IObservable<HarpMessage> source, int address, bool allowErrors)
        {
            return source.Where(message => message.IsMatch(address) && (!message.Error || allowErrors));
        }

        /// <summary>
        /// Filters the elements of an observable sequence of Harp messages based
        /// on their address and message type.
        /// </summary>
        /// <param name="source">The observable sequence whose messages to filter.</param>
        /// <param name="address">The address to test for a match.</param>
        /// <param name="messageType">The message type to test for a match.</param>
        /// <returns>
        /// An observable sequence that contains messages from the input sequence that
        /// match the specified <paramref name="address"/> and <paramref name="messageType"/>.
        /// </returns>
        public static IObservable<HarpMessage> Where(this IObservable<HarpMessage> source, int address, MessageType messageType)
        {
            return source.Where(message => message.IsMatch(address, messageType) && !message.Error);
        }

        /// <summary>
        /// Filters the elements of an observable sequence of Harp messages based
        /// on their address and message type.
        /// </summary>
        /// <param name="source">The observable sequence whose messages to filter.</param>
        /// <param name="address">The address to test for a match.</param>
        /// <param name="messageType">The message type to test for a match.</param>
        /// <param name="allowErrors"><c>true</c> to allow error messages in the filter; otherwise, <c>false</c>.</param>
        /// <returns>
        /// An observable sequence that contains messages from the input sequence that
        /// match the specified <paramref name="address"/> and <paramref name="messageType"/>.
        /// </returns>
        public static IObservable<HarpMessage> Where(this IObservable<HarpMessage> source, int address, MessageType messageType, bool allowErrors)
        {
            return source.Where(message => message.IsMatch(address, messageType) && (!message.Error || allowErrors));
        }
    }
}

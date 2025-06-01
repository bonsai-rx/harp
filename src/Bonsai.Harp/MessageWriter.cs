using System;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Bonsai.IO;
using SystemPath = System.IO.Path;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that writes each Harp message in the sequence
    /// to a raw binary file.
    /// </summary>
    [Description("Writes each Harp message in the sequence to a raw binary file.")]
    public class MessageWriter : FileSink<HarpMessage, BinaryWriter>
    {
        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [Description("Specifies how the message filter will use the matching criteria.")]
        public FilterType FilterType { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the expected message type. If no value is
        /// specified, all messages will be accepted.
        /// </summary>
        [Description("Specifies the expected message type. If no value is specified, all messages will be accepted.")]
        public MessageType? MessageType { get; set; }

        bool IsAccepted(HarpMessage input)
        {
            var messageType = MessageType;
            var includeMatch = FilterType == FilterType.Include;
            return !messageType.HasValue || (input.MessageType == messageType.GetValueOrDefault()) == includeMatch;
        }

        /// <summary>
        /// Creates a binary writer over the specified file that will be responsible
        /// for handling the input elements.
        /// </summary>
        /// <param name="fileName">The name of the file on which the elements should be written.</param>
        /// <param name="input">The first input element that needs to be pushed into the file.</param>
        /// <returns>The writer that will be used to push elements into the file.</returns>
        protected override BinaryWriter CreateWriter(string fileName, HarpMessage input)
        {
            if (IsAccepted(input))
            {
                var stream = new FileStream(fileName, Overwrite ? FileMode.Create : FileMode.CreateNew);
                return new BinaryWriter(stream);
            }
            else return null;
        }

        /// <summary>
        /// Writes a new Harp message to the raw binary output stream.
        /// </summary>
        /// <param name="writer">
        /// A <see cref="BinaryWriter"/> object used to write binary message data to
        /// the output stream.
        /// </param>
        /// <param name="input">
        /// The Harp message containing the binary data to write into the output
        /// stream.
        /// </param>
        protected override void Write(BinaryWriter writer, HarpMessage input)
        {
            if (writer != null && IsAccepted(input))
            {
                writer.Write(input.MessageBytes);
            }
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated
        /// from the common group register address.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique
        /// register address.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the
        /// Harp messages in each group to the corresponding file.
        /// </returns>
        public IObservable<IGroupedObservable<int, HarpMessage>> Process(IObservable<IGroupedObservable<int, HarpMessage>> source)
        {
            return Process(source, address => address.ToString());
        }

        /// <summary>
        /// Writes each Harp message in the sequence of observable groups to the
        /// corresponding binary file, where the name of each file is generated
        /// from the common group register name.
        /// </summary>
        /// <param name="source">
        /// A sequence of observable groups, each of which corresponds to a unique
        /// register type.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing the
        /// Harp messages in each group to the corresponding file.
        /// </returns>
        public IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<IGroupedObservable<Type, HarpMessage>> source)
        {
            return Process(source, type => type.Name);
        }

        private IObservable<IGroupedObservable<TKey, HarpMessage>> Process<TKey>(
            IObservable<IGroupedObservable<TKey, HarpMessage>> source,
            Func<TKey, string> suffixSelector)
        {
            var basePath = FileName;
            var directory = SystemPath.GetDirectoryName(basePath);
            var fileName = SystemPath.GetFileNameWithoutExtension(basePath);
            var extension = SystemPath.GetExtension(basePath);
            basePath = SystemPath.Combine(directory, fileName);
            return Observable.Create<IGroupedObservable<TKey, HarpMessage>>(observer =>
            {
                var sourceDisposable = new CompositeDisposable();
                var refCountDisposable = new RefCountDisposable(sourceDisposable);
                var sourceObserver = Observer.Create<IGroupedObservable<TKey, HarpMessage>>(
                    group =>
                    {
                        var path = $"{basePath}_{suffixSelector(group.Key)}{extension}";
                        var sink = Process(group, message => message, path).Publish().RefCount();
                        group = new GroupedObservable<TKey, HarpMessage>(group.Key, sink, refCountDisposable);

                        var groupDisposable = new SingleAssignmentDisposable();
                        var groupObserver = Observer.Create<HarpMessage>(
                            _ => { },
                            observer.OnError,
                            () =>
                            {
                                groupDisposable.Dispose();
                                sourceDisposable.Remove(groupDisposable);
                            });
                        groupDisposable.Disposable = sink.SubscribeSafe(groupObserver);
                        sourceDisposable.Add(groupDisposable);
                        observer.OnNext(group);
                    },
                    observer.OnError,
                    observer.OnCompleted);
                sourceDisposable.Add(source.SubscribeSafe(sourceObserver));
                return refCountDisposable;
            });
        }

        class GroupedObservable<TKey, TElement> : IGroupedObservable<TKey, TElement>
        {
            public GroupedObservable(TKey key, IObservable<TElement> elements, RefCountDisposable refCount)
            {
                Key = key;
                Elements = elements;
                RefCount = refCount;
            }

            public TKey Key { get; }

            private IObservable<TElement> Elements { get; }

            private RefCountDisposable RefCount { get; }

            public IDisposable Subscribe(IObserver<TElement> observer)
            {
                var refDisposable = RefCount.GetDisposable();
                var subscription = Elements.SubscribeSafe(observer);
                return new CompositeDisposable(refDisposable, subscription);
            }
        }
    }
}

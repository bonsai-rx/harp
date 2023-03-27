using System.ComponentModel;
using System.IO;
using Bonsai.IO;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that writes each Harp message in the sequence
    /// to a raw binary output stream.
    /// </summary>
    [Description("Writes each Harp message in the sequence to a raw binary output stream.")]
    public class MessageWriter : StreamSink<HarpMessage, BinaryWriter>
    {
        /// <summary>
        /// Creates a binary writer over the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream on which the elements should be written.</param>
        /// <returns>
        /// A <see cref="BinaryWriter"/> object used to write binary array data
        /// into the stream.
        /// </returns>
        protected override BinaryWriter CreateWriter(Stream stream)
        {
            return new BinaryWriter(stream);
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
            writer.Write(input.MessageBytes);
        }
    }
}

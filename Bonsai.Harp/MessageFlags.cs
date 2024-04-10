using System;

namespace Bonsai.Harp
{
    /// <summary>
    /// Specifies optional flags associated with a Harp message.
    /// </summary>
    [Flags]
    public enum MessageFlags : byte
    {
        /// <summary>
        /// Indicates that the message is an error message.
        /// </summary>
        Error = 0x08,

        /// <summary>
        /// Indicates that the message is a cancellation message.
        /// </summary>
        Cancellation = 0x10
    }
}

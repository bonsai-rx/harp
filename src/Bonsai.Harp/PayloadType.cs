using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Specifies the type of data available in the message payload.
    /// </summary>
    [TypeConverter(typeof(PayloadTypeConverter))]
    public enum PayloadType : byte
    {
        /// <summary>
        /// Specifies an unsigned 8-bits payload.
        /// </summary>
        U8 = (0x00 | 0x00 | 1),

        /// <summary>
        /// Specifies a signed 8-bits payload.
        /// </summary>
        S8 = (0x80 | 0x00 | 1),

        /// <summary>
        /// Specifies an unsigned 16-bits payload.
        /// </summary>
        U16 = (0x00 | 0x00 | 2),

        /// <summary>
        /// Specifies a signed 16-bits payload.
        /// </summary>
        S16 = (0x80 | 0x00 | 2),

        /// <summary>
        /// Specifies an unsigned 32-bits payload.
        /// </summary>
        U32 = (0x00 | 0x00 | 4),

        /// <summary>
        /// Specifies a signed 32-bits payload.
        /// </summary>
        S32 = (0x80 | 0x00 | 4),

        /// <summary>
        /// Specifies an unsigned 64-bits payload.
        /// </summary>
        U64 = (0x00 | 0x00 | 8),

        /// <summary>
        /// Specifies a signed 64-bits payload.
        /// </summary>
        S64 = (0x80 | 0x00 | 8),

        /// <summary>
        /// Specifies a single-precision 32-bits floating-point payload.
        /// </summary>
        Float = (0x00 | 0x40 | 4),

        /// <summary>
        /// Specifies the payload contains time information.
        /// </summary>
        Timestamp = 0x10,

        /// <summary>
        /// Specifies a timestamped unsigned 8-bits payload.
        /// </summary>
        TimestampedU8 = (Timestamp | U8),

        /// <summary>
        /// Specifies a timestamped signed 8-bits payload.
        /// </summary>
        TimestampedS8 = (Timestamp | S8),

        /// <summary>
        /// Specifies a timestamped unsigned 16-bits payload.
        /// </summary>
        TimestampedU16 = (Timestamp | U16),

        /// <summary>
        /// Specifies a timestamped signed 16-bits payload.
        /// </summary>
        TimestampedS16 = (Timestamp | S16),

        /// <summary>
        /// Specifies a timestamped unsigned 32-bits payload.
        /// </summary>
        TimestampedU32 = (Timestamp | U32),

        /// <summary>
        /// Specifies a timestamped signed 32-bits payload.
        /// </summary>
        TimestampedS32 = (Timestamp | S32),

        /// <summary>
        /// Specifies a timestamped unsigned 64-bits payload.
        /// </summary>
        TimestampedU64 = (Timestamp | U64),

        /// <summary>
        /// Specifies a timestamped signed 64-bits payload.
        /// </summary>
        TimestampedS64 = (Timestamp | S64),

        /// <summary>
        /// Specifies a timestamped single-precision 32-bits floating-point payload.
        /// </summary>
        TimestampedFloat = (Timestamp | Float)
    }
}

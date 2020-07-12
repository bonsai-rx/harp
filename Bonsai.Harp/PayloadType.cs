using System.ComponentModel;

namespace Bonsai.Harp
{
    [TypeConverter(typeof(PayloadTypeConverter))]
    public enum PayloadType : byte
    {
        U8 = (0x00 | 0x00 | 1),
        S8 = (0x80 | 0x00 | 1),
        U16 = (0x00 | 0x00 | 2),
        S16 = (0x80 | 0x00 | 2),
        U32 = (0x00 | 0x00 | 4),
        S32 = (0x80 | 0x00 | 4),
        U64 = (0x00 | 0x00 | 8),
        S64 = (0x80 | 0x00 | 8),
        Float = (0x00 | 0x40 | 4),
        Timestamp = 0x10,
        TimestampedU8 = (Timestamp | U8),
        TimestampedS8 = (Timestamp | S8),
        TimestampedU16 = (Timestamp | U16),
        TimestampedS16 = (Timestamp | S16),
        TimestampedU32 = (Timestamp | U32),
        TimestampedS32 = (Timestamp | S32),
        TimestampedU64 = (Timestamp | U64),
        TimestampedS64 = (Timestamp | S64),
        TimestampedFloat = (Timestamp | Float)
    }
}

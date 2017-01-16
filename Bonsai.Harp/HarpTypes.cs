using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    [Flags]
    public enum HarpTypes : byte
    {
        U8 = (0x00 | 0x00 | 1),
        I8 = (0x80 | 0x00 | 1),
        U16 = (0x00 | 0x00 | 2),
        I16 = (0x80 | 0x00 | 2),
        U32 = (0x00 | 0x00 | 4),
        I32 = (0x80 | 0x00 | 4),
        U64 = (0x00 | 0x00 | 8),
        I64 = (0x80 | 0x00 | 8),
        Float = (0x00 | 0x40 | 4),
        Timestamp = 0x10
    }
}

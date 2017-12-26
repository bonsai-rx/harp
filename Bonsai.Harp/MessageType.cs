using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    public enum MessageType : byte
    {
        Read = 0x01,
        Write = 0x02,
        Event = 0x03
    }
}

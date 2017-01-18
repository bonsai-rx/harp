using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp.Devices
{
    public enum DeviceEvent : byte
    {
        /* Event: TIMESTAMP_SECOND */
        EVT_Timestamp = 0,
        EVT_TimestampRaw,
    }
}

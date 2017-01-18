using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp.Devices
{
    public enum SynchronizerEvent : byte
    {
        /* Event: INPUTS_STATE */
        EVT0_Inputs = 0,
        EVT0_InputsRaw,

        EVT0_Input0,
        EVT0_Input1,
        EVT0_Input2,
        EVT0_Input3,
        EVT0_Input4,
        EVT0_Input5,
        EVT0_Input6,
        EVT0_Input7,
        EVT0_Input8,
        EVT0_Output0,
        EVT0_Address,
    }
}

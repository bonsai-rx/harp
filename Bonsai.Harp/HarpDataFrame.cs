using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    public class HarpDataFrame
    {
        const byte ErrorMask = 0x08;

        internal HarpDataFrame(byte[] message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            Message = message;
        }

        public MessageId Id
        {
            get { return (MessageId)(Message[0] & ~ErrorMask); }
        }

        public int Address
        {
            get { return Message[2]; }
        }

        public int Port
        {
            get { return Message[3]; }
        }

        public bool Error
        {
            get { return (Message[0] & ErrorMask) != 0; }
        }

        public byte[] Message { get; private set; }
    }
}

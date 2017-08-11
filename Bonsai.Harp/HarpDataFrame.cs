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

        public HarpDataFrame(params byte[] message)
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

        public static HarpDataFrame UpdateChesksum(HarpDataFrame frame)
        {
            var checksum = Checksum(frame);
            frame.Message[frame.Message.Length - 1] = Checksum(frame);
            return frame;
        }

        public static byte Checksum(HarpDataFrame frame)
        {
            return Checksum(frame.Message, frame.Message.Length - 1);
        }

        public static byte Checksum(params byte[] message)
        {
            return Checksum(message, message.Length);
        }

        static byte Checksum(byte[] message, int count)
        {
            var checksum = (byte)0;
            for (int i = 0; i < message.Length; i++)
            {
                checksum += message[i];
            }
            return checksum;
        }
    }
}

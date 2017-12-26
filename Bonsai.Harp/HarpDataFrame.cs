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

        public HarpDataFrame(bool updateChecksum, params byte[] message)
            : this(message)
        {
            if (updateChecksum)
            {
                message[message.Length - 1] = GetChecksum(message, message.Length - 1);
            }
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

        public PayloadType PayloadType
        {
            get { return (PayloadType)Message[4]; }
        }

        public bool Error
        {
            get { return (Message[0] & ErrorMask) != 0; }
        }

        public bool IsValid
        {
            get
            {
                var messageId = Id;
                var payloadType = PayloadType;
                var sizeOfType = (int)payloadType & 0x0F;
                var payloadArrayLength = (Message.Length - 10) / sizeOfType;

                if ((messageId != MessageId.Write) &&
                    (messageId != MessageId.Read) &&
                    (messageId != MessageId.Event) &&
                    ((byte)messageId != ((byte)MessageId.Write | ErrorMask)) &&
                    ((byte)messageId != ((byte)MessageId.Read | ErrorMask)))
                {
                    return false;
                }

                /* Check if the size of type is correct */
                if ((sizeOfType != 1) && (sizeOfType != 2) && (sizeOfType != 4) && (sizeOfType != 8))
                {
                    return false;
                }

                /* Check if the payload length is an integer number */
                if ((payloadArrayLength % 1) != 0)
                {
                    return false;
                }

                /* Bit 0x20 can't be high */
                if (((int)payloadType & 0x20) == 0x20)
                {
                    return false;
                }

                if (GetChecksum() != Message[Message.Length - 1])
                {
                    return false;
                }

                return true;
            }
        }

        public byte[] Message { get; private set; }

        public byte GetChecksum()
        {
            return GetChecksum(Message, Message.Length - 1);
        }

        static byte GetChecksum(byte[] message, int count)
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

using System;
using System.IO;

namespace Bonsai.Harp
{
    class BufferedStream
    {
        readonly Stream serialStream;
        readonly byte[] readBuffer;
        int readOffset;
        int writeOffset;

        public BufferedStream(Stream stream, int readBufferSize)
        {
            serialStream = stream;
            readBuffer = new byte[readBufferSize * 2];
        }

        public int BytesToRead
        {
            get
            {
                if (readOffset > writeOffset)
                {
                    return readBuffer.Length - readOffset + writeOffset;
                }
                else return writeOffset - readOffset;
            }
        }

        public int ReadByte()
        {
            if (readOffset == writeOffset) return -1;
            var result = readBuffer[readOffset];
            readOffset = (readOffset + 1) % readBuffer.Length;
            return result;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = 0;
            if (readOffset > writeOffset)
            {
                bytesRead = Math.Min(readBuffer.Length - readOffset, count);
                Array.Copy(readBuffer, readOffset, buffer, offset, bytesRead);
                readOffset = (readOffset + bytesRead) % readBuffer.Length;
                count -= bytesRead;
                offset += bytesRead;
                if (count == 0) return bytesRead;
            }

            var remaining = Math.Min(writeOffset - readOffset, count);
            Array.Copy(readBuffer, readOffset, buffer, offset, remaining);
            readOffset = (readOffset + remaining) % readBuffer.Length;
            return bytesRead + remaining;
        }

        public void Seek(int offset)
        {
            if (offset < 0)
            {
                readOffset = (readOffset + readBuffer.Length + offset) % readBuffer.Length;
            }
            else readOffset = (readOffset + offset) % readBuffer.Length;
        }

        public int PushBytes(int count)
        {
            var bytesWritten = 0;
            if (writeOffset >= readOffset)
            {
                bytesWritten = Math.Min(readBuffer.Length - writeOffset, count);
                bytesWritten = serialStream.Read(readBuffer, writeOffset, bytesWritten);
                writeOffset = (writeOffset + bytesWritten) % readBuffer.Length;
                count -= bytesWritten;
                if (count == 0) return bytesWritten;
            }

            var remaining = Math.Min(readOffset - writeOffset, count);
            remaining = serialStream.Read(readBuffer, writeOffset, remaining);
            writeOffset = (writeOffset + remaining) % readBuffer.Length;
            return bytesWritten + remaining;
        }
    }
}

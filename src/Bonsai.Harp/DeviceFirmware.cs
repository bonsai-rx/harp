using System;
using System.IO;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents a hardware control firmware image which can be uploaded into a Harp device.
    /// </summary>
    public sealed class DeviceFirmware
    {
        const int DefaultPageSize = 512;

        private DeviceFirmware(FirmwareMetadata metadata, byte[] data)
        {
            Metadata = metadata;
            Data = data;
        }

        /// <summary>
        /// Gets information about the firmware version and supported devices on which it can be installed.
        /// </summary>
        public FirmwareMetadata Metadata { get; private set; }

        /// <summary>
        /// Gets the binary representation of the firmware to be installed on the device.
        /// </summary>
        public byte[] Data { get; private set; }

        static char ReadChar(StreamReader stream, char[] hexDigit)
        {
            var read = stream.Read(hexDigit, 0, 1);
            if (read != 1) throw new ArgumentException("Invalid hex code specification found in hex stream.", nameof(stream));
            return hexDigit[0];
        }

        static byte ReadHexByte(StreamReader stream, char[] hexDigit)
        {
            var read = stream.Read(hexDigit, 0, 2);
            if (read != 2) throw new ArgumentException("Invalid hex code specification found in hex stream.", nameof(stream));
            return Convert.ToByte(new string(hexDigit, 0, 2), 16);
        }

        static ushort ReadHexUInt16(StreamReader stream, char[] hexDigit)
        {
            var read = stream.Read(hexDigit, 0, 4);
            if (read != 4) throw new ArgumentException("Invalid hex code specification found in hex stream.", nameof(stream));
            return Convert.ToUInt16(new string(hexDigit, 0, 4), 16);
        }

        static int ReadHexData(StreamReader stream, char[] hexDigit, ref short[] data, int offset, int count, int pageSize)
        {
            var end = offset + count;
            if (end > data.Length)
            {
                var numPages = end / pageSize + (end % pageSize > 0 ? 1 : 0);
                Expand(ref data, numPages * pageSize);
            }

            var sum = 0;
            for (int i = offset; i < end; i++)
            {
                data[i] = ReadHexByte(stream, hexDigit);
                sum += data[i];
            }
            return sum;
        }

        static int Checksum(ushort value)
        {
            return (byte)value + (byte)(value >> 8);
        }

        static void Expand(ref short[] data, int newSize)
        {
            var offset = data.Length;
            Array.Resize(ref data, newSize);
            for (int i = offset; i < data.Length; i++)
            {
                data[i] = -1;
            }
        }

        /// <summary>
        /// Creates a <see cref="DeviceFirmware"/> object from the specified file in Intel HEX format
        /// using the default page size.
        /// </summary>
        /// <param name="path">The name of the file from which to create the <see cref="DeviceFirmware"/>.</param>
        /// <returns>
        /// A new <see cref="DeviceFirmware"/> object representing the extracted binary firware blob,
        /// together with the metadata extracted from the firmware file name.
        /// </returns>
        public static DeviceFirmware FromFile(string path)
        {
            return FromFile(path, DefaultPageSize);
        }

        /// <summary>
        /// Creates a <see cref="DeviceFirmware"/> object from the specified file in Intel HEX format
        /// and a specified page size.
        /// </summary>
        /// <param name="path">The name of the file from which to create the <see cref="DeviceFirmware"/>.</param>
        /// <param name="pageSize">The size of the memory blocks used to upload the device firmware.</param>
        /// <returns>
        /// A new <see cref="DeviceFirmware"/> object representing the extracted binary firware blob,
        /// together with the metadata extracted from the firmware file name.
        /// </returns>
        public static DeviceFirmware FromFile(string path, int pageSize)
        {
            var metadata = Path.GetFileNameWithoutExtension(path);
            using (var stream = File.OpenRead(path))
            {
                return FromStream(metadata, stream, pageSize);
            }
        }

        /// <summary>
        /// Creates a <see cref="DeviceFirmware"/> object extracted from the specified ASCII
        /// stream in Intel HEX format, the specified metadata string and page size.
        /// </summary>
        /// <param name="metadata">The firmware metadata encoded in a text string representation.</param>
        /// <param name="stream">The ASCII stream in Intel HEX format from which to extract the device firmware.</param>
        /// <param name="pageSize">The size of the memory blocks used to upload the device firmware.</param>
        /// <returns>
        /// A new <see cref="DeviceFirmware"/> object representing the extracted binary firware blob,
        /// together with the metadata extracted from the firmware file name.
        /// </returns>
        public static DeviceFirmware FromStream(string metadata, Stream stream, int pageSize)
        {
            const char StartCode = ':';
            var firmwareMetadata = FirmwareMetadata.Parse(metadata);
            using (var reader = new StreamReader(stream))
            {
                var lineNumber = 0;
                var baseAddress = 0;
                var hexDigit = new char[4];
                var data = new short[0];
                Expand(ref data, pageSize);
                while (!reader.EndOfStream)
                {
                    if (ReadChar(reader, hexDigit) != StartCode)
                    {
                        throw new ArgumentException($"{lineNumber}: Invalid record start code found in hex stream.");
                    }

                    var sum = 0;
                    var count = ReadHexByte(reader, hexDigit);
                    var address = ReadHexUInt16(reader, hexDigit);
                    var recordType = (RecordType)ReadHexByte(reader, hexDigit);
                    switch (recordType)
                    {
                        case RecordType.Data:
                            sum = ReadHexData(reader, hexDigit, ref data, baseAddress + address, count, pageSize);
                            break;
                        case RecordType.EndOfFile: break;
                        case RecordType.ExtendedSegmentAddress:
                            if (count != 2) throw new ArgumentException($"{lineNumber}: Invalid extended segment address payload found in hex stream.");
                            var segmentAddress = ReadHexUInt16(reader, hexDigit);
                            baseAddress = segmentAddress * 16;
                            sum = Checksum(segmentAddress);
                            break;
                        case RecordType.ExtendedLinearAddress:
                            if (count != 2) throw new ArgumentException($"{lineNumber}: Invalid extended linear address payload found in hex stream.");
                            var extendedAddress = ReadHexUInt16(reader, hexDigit);
                            baseAddress = extendedAddress << 16;
                            sum = Checksum(extendedAddress);
                            break;
                        case RecordType.StartLinearAddress:
                        case RecordType.StartSegmentAddress: throw new NotSupportedException($"{lineNumber}: Unsupported record type found in hex stream.");
                        default: throw new ArgumentException($"{lineNumber}: Invalid record type found in hex stream.");
                    }

                    sum = (byte)(sum + count + Checksum(address) + (byte)recordType);
                    var checksum = sum + ReadHexByte(reader, hexDigit);
                    if ((byte)checksum != 0)
                    {
                        throw new ArgumentException($"{lineNumber}: Invalid data checksum found in hex stream.");
                    }

                    reader.ReadLine();
                    lineNumber++;
                }

                var byteCode = Array.ConvertAll(data, value => (byte)value);
                return new DeviceFirmware(firmwareMetadata, byteCode);
            }
        }

        enum RecordType : byte
        {
            Data = 0,
            EndOfFile = 1,
            ExtendedSegmentAddress = 2,
            StartSegmentAddress = 3,
            ExtendedLinearAddress = 4,
            StartLinearAddress = 5
        }
    }
}

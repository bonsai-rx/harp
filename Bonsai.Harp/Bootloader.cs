using System;
using System.IO;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides asynchronous operations to update a device firmware using the dedicated bootloader protocol.
    /// </summary>
    public static class Bootloader
    {
        const int HeaderSize = 15;
        const int FlushDelayMilliseconds = 500;

        const int WritePage = 0x0;
        const int ReadPageSize = 0x66;
        const int ExitBootloader = 0x77;

        const int NoError = 0;
        const int UndefinedError = 1;
        const int InvalidAddress = 2;
        const int InvalidDataLength = 3;

        /// <summary>
        /// Asynchronously updates the firmware of the Harp device on the specified port.
        /// </summary>
        /// <param name="portName">The name of the serial port used to communicate with the Harp device.</param>
        /// <param name="firmware">The binary firmware image to upload to the device.</param>
        /// <param name="progress">The optional <see cref="IProgress{Int32}"/> object used to report update progress.</param>
        /// <returns>
        /// The task object representing the asynchronous firmware update operation.
        /// </returns>
        public static Task UpdateFirmwareAsync(string portName, DeviceFirmware firmware, IProgress<int> progress = default)
        {
            return UpdateFirmwareAsync(portName, firmware, forceUpdate: false, progress: progress);
        }

        /// <summary>
        /// Asynchronously updates the firmware of the Harp device on the specified port.
        /// </summary>
        /// <param name="portName">The name of the serial port used to communicate with the Harp device.</param>
        /// <param name="firmware">The binary firmware image to upload to the device.</param>
        /// <param name="forceUpdate">
        /// <b>true</b> to indicate that the firmware should be uploaded even if the device reports unsupported hardware,
        /// or is in bootloader mode; <b>false</b> to throw an exception if the firmware is not supported, or the device
        /// is in an invalid state.
        /// </param>
        /// <param name="progress">The optional <see cref="IProgress{Int32}"/> object used to report update progress.</param>
        /// <returns>
        /// The task object representing the asynchronous firmware update operation.
        /// </returns>
        public static async Task UpdateFirmwareAsync(string portName, DeviceFirmware firmware, bool forceUpdate, IProgress<int> progress = default)
        {
            var flushDelay = TimeSpan.FromMilliseconds(FlushDelayMilliseconds);
            using (var device = new AsyncDevice(portName))
            {
                try
                {
                    var hardwareVersion = await device.ReadHardwareVersionAsync().WithTimeout(FlushDelayMilliseconds);
                    var deviceName = await device.ReadDeviceNameAsync().WithTimeout(FlushDelayMilliseconds);
                    if (!firmware.Metadata.Supports(deviceName, hardwareVersion) && !forceUpdate)
                    {
                        throw new ArgumentException("The specified firmware is not supported.", nameof(firmware));
                    }

                    const byte BootEeprom = 0x80;
                    const byte BootDefault = 0x40;
                    const byte ResetDefault = 0x1;
                    const byte ResetEeprom = 0x2;
                    var reset = await device.ReadByteAsync(DeviceRegisters.ResetDevice);
                    if ((reset & BootEeprom) != 0)
                    {
                        await device.WriteByteAsync(DeviceRegisters.ResetDevice, ResetEeprom);
                    }
                    else if ((reset & BootDefault) != 0)
                    {
                        await device.WriteByteAsync(DeviceRegisters.ResetDevice, ResetDefault);
                    }
                    else throw new HarpException("The device is in an unexpected boot mode.");
                    await Observable.Timer(flushDelay);
                    progress?.Report(20);
                }
                catch (TimeoutException)
                {
                    if (!forceUpdate)
                    {
                        throw;
                    }
                }
            }

            const int DefaultBaudRate = 1000000;
            using (var bootloader = new SerialPort(portName, DefaultBaudRate, Parity.None, 8, StopBits.One))
            {
                bootloader.Handshake = Handshake.None;
                bootloader.Open();
                await Observable.Timer(flushDelay);
                var pageSize = await ReadPageSizeAsync(bootloader.BaseStream);
                progress?.Report(40);

                var bytesWritten = 0;
                var reportSize = pageSize * 8;
                var dataMessage = new byte[pageSize + HeaderSize];
                while (bytesWritten < firmware.Data.Length)
                {
                    CreateBootloaderMessage(dataMessage, WritePage, bytesWritten, firmware.Data, bytesWritten, pageSize);
                    await BootloaderCommandAsync(bootloader.BaseStream, dataMessage);
                    bytesWritten += pageSize;
                    if (bytesWritten % reportSize == 0)
                    {
                        progress?.Report(40 + bytesWritten * 50 / firmware.Data.Length);
                    }
                }

                progress?.Report(90);
                CreateBootloaderMessage(dataMessage, ExitBootloader, 0, firmware.Data, 0, pageSize);
                await BootloaderCommandAsync(bootloader.BaseStream, dataMessage);
                progress?.Report(100);
            };
        }

        static async Task<T> WithTimeout<T>(this Task<T> task, int millisecondsDelay)
        {
            if (await Task.WhenAny(task, Task.Delay(millisecondsDelay)) == task)
            {
                return task.Result;
            }
            else throw new TimeoutException("There was a timeout while awaiting the device response.");
        }

        static ushort GetMessageChecksum(byte[] messageBytes)
        {
            var checksum = (ushort)0;
            unchecked
            {
                for (int i = 0; i < messageBytes.Length - 2; i++)
                {
                    checksum += messageBytes[i];
                }
            }
            return checksum;
        }

        static bool IsValidChecksum(byte[] messageBytes)
        {
            var checksum = GetMessageChecksum(messageBytes);
            var messageChecksum = (messageBytes[messageBytes.Length - 1] << 8) + messageBytes[messageBytes.Length - 2];
            return checksum == messageChecksum;
        }

        static void CreateBootloaderMessage(byte[] messageBytes, int opcode, int address, byte[] data, int offset, int count)
        {
            messageBytes[0] = 1;
            messageBytes[1] = 2;
            messageBytes[2] = 3;
            messageBytes[3] = (byte)opcode;
            messageBytes[4] = 0; //error
            messageBytes[5] = (byte)address;
            messageBytes[6] = (byte)(address >> 8);
            messageBytes[7] = (byte)(address >> 16);
            messageBytes[8] = (byte)(address >> 24);
            messageBytes[9] = (byte)count;
            messageBytes[10] = (byte)(count >> 8);
            messageBytes[11] = (byte)(count >> 16);
            messageBytes[12] = (byte)(count >> 24);
            Array.Copy(data, offset, messageBytes, 13, count);
            var checksum = GetMessageChecksum(messageBytes);
            messageBytes[messageBytes.Length - 2] = (byte)checksum;
            messageBytes[messageBytes.Length - 1] = (byte)(checksum >> 8);
        }

        static byte[] CreateBootloaderMessage(int opcode, int address, params byte[] data)
        {
            var messageBytes = new byte[data.Length + HeaderSize];
            CreateBootloaderMessage(messageBytes, opcode, address, data, 0, data.Length);
            return messageBytes;
        }

        static async Task<int> ReadPageSizeAsync(Stream stream)
        {
            var message = CreateBootloaderMessage(ReadPageSize, address: 0);
            await BootloaderCommandAsync(stream, message);
            return BitConverter.ToInt32(message, startIndex: 9);
        }

        static async Task BootloaderCommandAsync(Stream stream, byte[] message)
        {
            var bytesRead = 0;
            await stream.WriteAsync(message, 0, message.Length);
            while (bytesRead < message.Length)
            {
                bytesRead += await stream.ReadAsync(message, bytesRead, message.Length - bytesRead)
                                         .WithTimeout(FlushDelayMilliseconds);
            }

            if (bytesRead != message.Length)
            {
                throw new HarpException("The device responded with an invalid buffer length.");
            }

            if (!IsValidChecksum(message))
            {
                throw new HarpException("The device responded with an invalid response checksum.");
            }

            switch (message[4])
            {
                case UndefinedError: throw new HarpException("The device reported an undefined error while updating the bootloader logic.");
                case InvalidAddress: throw new HarpException("The device reported an invalid address while updating the bootloader logic.");
                case InvalidDataLength: throw new HarpException("The device reported an invalid data length while writing the bootloader page.");
                case NoError:
                default:
                    break;
            }
        }
    }
}

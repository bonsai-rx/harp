using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an asynchronous API to configure and interface with Harp devices.
    /// </summary>
    public class AsyncDevice : IDisposable
    {
        readonly bool _leaveOpen;
        readonly SerialTransport transport;
        readonly Subject<HarpMessage> response;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDevice"/> class on
        /// the specified port.
        /// </summary>
        /// <param name="portName">The name of the serial port used to communicate with the Harp device.</param>
        public AsyncDevice(string portName)
        {
            response = new Subject<HarpMessage>();
            transport = new SerialTransport(portName, response);
            transport.IgnoreErrors = true;
            transport.Open();
        }

        internal AsyncDevice(string portName, bool leaveOpen)
            : this(portName)
        {
            _leaveOpen = leaveOpen;
        }

        internal SerialTransport Transport
        {
            get { return transport; }
        }

        /// <summary>
        /// Asynchronously reads the identity class of the device.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the identity class of the device.
        /// </returns>
        public async Task<int> ReadWhoAmIAsync()
        {
            return await ReadUInt16Async(WhoAmI.Address);
        }

        /// <summary>
        /// Asynchronously reads the hardware version of the device.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the hardware version of the device.
        /// </returns>
        public async Task<HarpVersion> ReadHardwareVersionAsync()
        {
            var major = await ReadByteAsync(HardwareVersionHigh.Address);
            var minor = await ReadByteAsync(HardwareVersionLow.Address);
            return new HarpVersion(major, minor);
        }

        /// <summary>
        /// Asynchronously reads the assembly version of the device.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the assembly version of the device.
        /// </returns>
        public async Task<int> ReadAssemblyVersionAsync()
        {
            return await ReadByteAsync(AssemblyVersion.Address);
        }

        /// <summary>
        /// Asynchronously reads the version of the Harp core implemented
        /// by the device firmware.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the version of the Harp core implemented by the device firmware.
        /// </returns>
        public async Task<HarpVersion> ReadCoreVersionAsync()
        {
            var major = await ReadByteAsync(CoreVersionHigh.Address);
            var minor = await ReadByteAsync(CoreVersionLow.Address);
            return new HarpVersion(major, minor);
        }

        /// <summary>
        /// Asynchronously reads the firmware version of the device.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the firmware version of the device.
        /// </returns>
        public async Task<HarpVersion> ReadFirmwareVersionAsync()
        {
            var major = await ReadByteAsync(FirmwareVersionHigh.Address);
            var minor = await ReadByteAsync(FirmwareVersionLow.Address);
            return new HarpVersion(major, minor);
        }

        /// <summary>
        /// Asynchronously reads the integral part of the system timestamp, in seconds.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the integral part of the system timestamp, in seconds.
        /// </returns>
        public async Task<uint> ReadTimestampSecondsAsync()
        {
            return await ReadUInt32Async(TimestampSeconds.Address);
        }

        /// <summary>
        /// Asynchronously reads the fractional part of the system timestamp, in microseconds.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the fractional part of the system timestamp, in microseconds.
        /// </returns>
        public async Task<ushort> ReadTimestampMicrosecondsAsync()
        {
            return await ReadUInt16Async(TimestampMicroseconds.Address);
        }

        /// <summary>
        /// Asynchronously reads the contents of the OperationControl register.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<OperationControlPayload> ReadOperationControlAsync()
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(OperationControl.Address));
            return OperationControl.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ResetDevice register.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<ResetFlags> ReadResetDeviceAsync()
        {
            return (ResetFlags)await ReadByteAsync(ResetDevice.Address);
        }

        /// <summary>
        /// Asynchronously reads the display name of the device.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the name of the device.
        /// </returns>
        public async Task<string> ReadDeviceNameAsync()
        {
            var deviceName = await CommandAsync(HarpCommand.ReadByte(DeviceName.Address));
            return DeviceName.GetPayload(deviceName);
        }

        /// <summary>
        /// Asynchronously reads the unique serial number of the device.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the unique serial number of the device.
        /// </returns>
        public async Task<int> ReadSerialNumberAsync()
        {
            return await ReadUInt16Async(SerialNumber.Address);
        }

        /// <summary>
        /// Asynchronously reads the configuration for the device synchronization clock.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the configuration for the device synchronization clock.
        /// </returns>
        public async Task<ClockConfigurationFlags> ReadClockConfigurationAsync()
        {
            return (ClockConfigurationFlags)await ReadByteAsync(ClockConfiguration.Address);
        }

        /// <summary>
        /// Asynchronously reads the value of an 8-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<byte> ReadByteAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(address));
            return reply.GetPayloadByte();
        }

        /// <summary>
        /// Asynchronously reads the value of an 8-bit unsigned integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<byte[]> ReadByteArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(address));
            return reply.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Asynchronously reads the value of an 8-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<sbyte> ReadSByteAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadSByte(address));
            return reply.GetPayloadSByte();
        }

        /// <summary>
        /// Asynchronously reads the value of an 8-bit signed integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<sbyte[]> ReadSByteArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadSByte(address));
            return reply.GetPayloadArray<sbyte>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 16-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<ushort> ReadUInt16Async(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(address));
            return reply.GetPayloadUInt16();
        }

        /// <summary>
        /// Asynchronously reads the value of a 16-bit unsigned integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<ushort[]> ReadUInt16ArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(address));
            return reply.GetPayloadArray<ushort>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 16-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<short> ReadInt16Async(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt16(address));
            return reply.GetPayloadInt16();
        }

        /// <summary>
        /// Asynchronously reads the value of a 16-bit signed integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<short[]> ReadInt16ArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt16(address));
            return reply.GetPayloadArray<short>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 32-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<uint> ReadUInt32Async(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(address));
            return reply.GetPayloadUInt32();
        }

        /// <summary>
        /// Asynchronously reads the value of a 32-bit unsigned integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<uint[]> ReadUInt32ArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(address));
            return reply.GetPayloadArray<uint>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 32-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<int> ReadInt32Async(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt32(address));
            return reply.GetPayloadInt32();
        }

        /// <summary>
        /// Asynchronously reads the value of a 32-bit signed integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<int[]> ReadInt32ArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt32(address));
            return reply.GetPayloadArray<int>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 64-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<ulong> ReadUInt64Async(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt64(address));
            return reply.GetPayloadUInt64();
        }

        /// <summary>
        /// Asynchronously reads the value of a 64-bit unsigned integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<ulong[]> ReadUInt64ArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt64(address));
            return reply.GetPayloadArray<ulong>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 64-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<long> ReadInt64Async(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt64(address));
            return reply.GetPayloadInt64();
        }

        /// <summary>
        /// Asynchronously reads the value of a 64-bit signed integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<long[]> ReadInt64ArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt64(address));
            return reply.GetPayloadArray<long>();
        }

        /// <summary>
        /// Asynchronously reads the value of a single-precision floating point register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<float> ReadSingleAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(address));
            return reply.GetPayloadSingle();
        }

        /// <summary>
        /// Asynchronously reads the value of a single-precision floating point array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<float[]> ReadSingleArrayAsync(int address)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(address));
            return reply.GetPayloadArray<float>();
        }

        /// <summary>
        /// Asynchronously updates the integral part of the system timestamp, in seconds.
        /// </summary>
        /// <param name="seconds">The value to be stored in the register.</param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteTimestampSecondsAsync(uint seconds)
        {
            await WriteUInt32Async(TimestampSeconds.Address, seconds);
        }

        /// <summary>
        /// Asynchronously sends a command to reset the device and restore or
        /// save non-volatile registers.
        /// </summary>
        /// <param name="reset">
        /// A value specifying whether to restore or save non-volatile registers.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous reset operation.
        /// </returns>
        public async Task WriteResetDeviceAsync(ResetFlags reset)
        {
            await WriteByteAsync(ResetDevice.Address, (byte)reset);
        }

        /// <summary>
        /// Asynchronously updates the display name of the device.
        /// </summary>
        /// <param name="name">
        /// A <see cref="string"/> containing the name of the device. The maximum length
        /// of the specified device name is 25 characters.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteDeviceNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The specified device name cannot be null or empty.", nameof(name));
            }

            await CommandAsync(DeviceName.FromPayload(MessageType.Write, name));
        }

        /// <summary>
        /// Asynchronously updates the configuration for the device synchronization clock.
        /// </summary>
        /// <param name="value">A value specifying configuration flags for the device synchronization clock.</param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteClockConfigurationAsync(ClockConfigurationFlags value)
        {
            await WriteByteAsync(ClockConfiguration.Address, (byte)value);
        }

        /// <summary>
        /// Asynchronously writes a value to an 8-bit unsigned integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteByteAsync(int address, byte value) => await CommandAsync(HarpCommand.WriteByte(address, value));

        /// <summary>
        /// Asynchronously writes an array of values to an 8-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteByteAsync(int address, params byte[] values) => await CommandAsync(HarpCommand.WriteByte(address, values));

        /// <summary>
        /// Asynchronously writes a value to an 8-bit signed integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteSByteAsync(int address, sbyte value) => await CommandAsync(HarpCommand.WriteSByte(address, value));

        /// <summary>
        /// Asynchronously writes an array of values to an 8-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteSByteAsync(int address, params sbyte[] values) => await CommandAsync(HarpCommand.WriteSByte(address, values));

        /// <summary>
        /// Asynchronously writes a value to a 16-bit unsigned integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt16Async(int address, ushort value) => await CommandAsync(HarpCommand.WriteUInt16(address, value));

        /// <summary>
        /// Asynchronously writes an array of values to a 16-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt16Async(int address, params ushort[] values) => await CommandAsync(HarpCommand.WriteUInt16(address, values));

        /// <summary>
        /// Asynchronously writes a value to a 16-bit signed integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt16Async(int address, short value) => await CommandAsync(HarpCommand.WriteInt16(address, value));

        /// <summary>
        /// Asynchronously writes an array of values to a 16-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt16Async(int address, params short[] values) => await CommandAsync(HarpCommand.WriteInt16(address, values));

        /// <summary>
        /// Asynchronously writes a value to a 32-bit unsigned integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt32Async(int address, uint value) => await CommandAsync(HarpCommand.WriteUInt32(address, value));

        /// <summary>
        /// Asynchronously writes an array of values to a 32-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt32Async(int address, params uint[] values) => await CommandAsync(HarpCommand.WriteUInt32(address, values));

        /// <summary>
        /// Asynchronously writes a value to a 32-bit signed integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt32Async(int address, int value) => await CommandAsync(HarpCommand.WriteInt32(address, value));

        /// <summary>
        /// Asynchronously writes an array of values to a 32-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt32Async(int address, params int[] values) => await CommandAsync(HarpCommand.WriteInt32(address, values));

        /// <summary>
        /// Asynchronously writes a value to a 64-bit unsigned integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt64Async(int address, ulong value) => await CommandAsync(HarpCommand.WriteUInt64(address, value));

        /// <summary>
        /// Asynchronously writes an array of values to a 64-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt64Async(int address, params ulong[] values) => await CommandAsync(HarpCommand.WriteUInt64(address, values));

        /// <summary>
        /// Asynchronously writes a value to a 64-bit signed integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt64Async(int address, long value) => await CommandAsync(HarpCommand.WriteInt64(address, value));

        /// <summary>
        /// Asynchronously writes an array of values to a 64-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt64Async(int address, params long[] values) => await CommandAsync(HarpCommand.WriteInt64(address, values));

        /// <summary>
        /// Asynchronously writes a value, or an array of values, to a single-precision floating point register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteSingleAsync(int address, params float[] values) => await CommandAsync(HarpCommand.WriteSingle(address, values));

        /// <summary>
        /// Sends a command to the Harp device and awaits the response as an asynchronous operation.
        /// </summary>
        /// <param name="command">The <see cref="HarpMessage"/> specifying the command to send.</param>
        /// <returns>
        /// The task object representing the asynchronous operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the message representing the response of the device to the asynchronous command.
        /// </returns>
        public async Task<HarpMessage> CommandAsync(HarpMessage command)
        {
            var reply = response.FirstAsync(message =>
            {
                var match = message.IsMatch(command.Address, command.MessageType);
                if (match && message.Error)
                {
                    throw new HarpException(message);
                }

                return match;
            }).GetAwaiter();

            transport.Write(command);
            return await reply;
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="AsyncDevice"/>.
        /// </summary>
        public void Dispose()
        {
            if (!_leaveOpen)
            {
                transport.Close();
            }
            response.Dispose();
        }
    }
}

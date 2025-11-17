using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an asynchronous API to configure and interface with Harp devices.
    /// </summary>
    public class AsyncDevice : IDisposable, IAsyncDisposable
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
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the identity class of the device.
        /// </returns>
        public async Task<int> ReadWhoAmIAsync(CancellationToken cancellationToken = default)
        {
            return await ReadUInt16Async(WhoAmI.Address, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the hardware version of the device.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the hardware version of the device.
        /// </returns>
        public async Task<HarpVersion> ReadHardwareVersionAsync(CancellationToken cancellationToken = default)
        {
            var major = await ReadByteAsync(HardwareVersionHigh.Address, cancellationToken);
            var minor = await ReadByteAsync(HardwareVersionLow.Address, cancellationToken);
            return new HarpVersion(major, minor);
        }

        /// <summary>
        /// Asynchronously reads the assembly version of the device.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the assembly version of the device.
        /// </returns>
        public async Task<int> ReadAssemblyVersionAsync(CancellationToken cancellationToken = default)
        {
            return await ReadByteAsync(AssemblyVersion.Address, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the version of the Harp core implemented
        /// by the device firmware.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the version of the Harp core implemented by the device firmware.
        /// </returns>
        public async Task<HarpVersion> ReadCoreVersionAsync(CancellationToken cancellationToken = default)
        {
            var major = await ReadByteAsync(CoreVersionHigh.Address, cancellationToken);
            var minor = await ReadByteAsync(CoreVersionLow.Address, cancellationToken);
            return new HarpVersion(major, minor);
        }

        /// <summary>
        /// Asynchronously reads the firmware version of the device.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the firmware version of the device.
        /// </returns>
        public async Task<HarpVersion> ReadFirmwareVersionAsync(CancellationToken cancellationToken = default)
        {
            var major = await ReadByteAsync(FirmwareVersionHigh.Address, cancellationToken);
            var minor = await ReadByteAsync(FirmwareVersionLow.Address, cancellationToken);
            return new HarpVersion(major, minor);
        }

        /// <summary>
        /// Asynchronously reads the integral part of the system timestamp, in seconds.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the integral part of the system timestamp, in seconds.
        /// </returns>
        public async Task<uint> ReadTimestampSecondsAsync(CancellationToken cancellationToken = default)
        {
            return await ReadUInt32Async(TimestampSeconds.Address, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the fractional part of the system timestamp, in microseconds.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the fractional part of the system timestamp, in microseconds.
        /// </returns>
        public async Task<ushort> ReadTimestampMicrosecondsAsync(CancellationToken cancellationToken = default)
        {
            return await ReadUInt16Async(TimestampMicroseconds.Address, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the contents of the OperationControl register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<OperationControlPayload> ReadOperationControlAsync(CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(OperationControl.Address), cancellationToken);
            return OperationControl.GetPayload(reply);
        }

        /// <summary>
        /// Asynchronously reads the contents of the ResetDevice register.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the register payload.
        /// </returns>
        public async Task<ResetFlags> ReadResetDeviceAsync(CancellationToken cancellationToken = default)
        {
            return (ResetFlags)await ReadByteAsync(ResetDevice.Address, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the display name of the device.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the name of the device.
        /// </returns>
        public async Task<string> ReadDeviceNameAsync(CancellationToken cancellationToken = default)
        {
            var deviceName = await CommandAsync(HarpCommand.ReadByte(DeviceName.Address), cancellationToken);
            return DeviceName.GetPayload(deviceName);
        }

        /// <summary>
        /// Asynchronously reads the unique serial number of the device.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the unique serial number of the device.
        /// </returns>
        public async Task<int> ReadSerialNumberAsync(CancellationToken cancellationToken = default)
        {
            return await ReadUInt16Async(SerialNumber.Address, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the configuration for the device synchronization clock.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the configuration for the device synchronization clock.
        /// </returns>
        public async Task<ClockConfigurationFlags> ReadClockConfigurationAsync(CancellationToken cancellationToken = default)
        {
            return (ClockConfigurationFlags)await ReadByteAsync(ClockConfiguration.Address, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the value of an 8-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<byte> ReadByteAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(address), cancellationToken);
            return reply.GetPayloadByte();
        }

        /// <summary>
        /// Asynchronously reads the value of an 8-bit unsigned integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<byte[]> ReadByteArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadByte(address), cancellationToken);
            return reply.GetPayloadArray<byte>();
        }

        /// <summary>
        /// Asynchronously reads the value of an 8-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<sbyte> ReadSByteAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSByte(address), cancellationToken);
            return reply.GetPayloadSByte();
        }

        /// <summary>
        /// Asynchronously reads the value of an 8-bit signed integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<sbyte[]> ReadSByteArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSByte(address), cancellationToken);
            return reply.GetPayloadArray<sbyte>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 16-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<ushort> ReadUInt16Async(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(address), cancellationToken);
            return reply.GetPayloadUInt16();
        }

        /// <summary>
        /// Asynchronously reads the value of a 16-bit unsigned integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<ushort[]> ReadUInt16ArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt16(address), cancellationToken);
            return reply.GetPayloadArray<ushort>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 16-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<short> ReadInt16Async(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt16(address), cancellationToken);
            return reply.GetPayloadInt16();
        }

        /// <summary>
        /// Asynchronously reads the value of a 16-bit signed integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<short[]> ReadInt16ArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt16(address), cancellationToken);
            return reply.GetPayloadArray<short>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 32-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<uint> ReadUInt32Async(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(address), cancellationToken);
            return reply.GetPayloadUInt32();
        }

        /// <summary>
        /// Asynchronously reads the value of a 32-bit unsigned integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<uint[]> ReadUInt32ArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt32(address), cancellationToken);
            return reply.GetPayloadArray<uint>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 32-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<int> ReadInt32Async(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt32(address), cancellationToken);
            return reply.GetPayloadInt32();
        }

        /// <summary>
        /// Asynchronously reads the value of a 32-bit signed integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<int[]> ReadInt32ArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt32(address), cancellationToken);
            return reply.GetPayloadArray<int>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 64-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<ulong> ReadUInt64Async(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt64(address), cancellationToken);
            return reply.GetPayloadUInt64();
        }

        /// <summary>
        /// Asynchronously reads the value of a 64-bit unsigned integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<ulong[]> ReadUInt64ArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadUInt64(address), cancellationToken);
            return reply.GetPayloadArray<ulong>();
        }

        /// <summary>
        /// Asynchronously reads the value of a 64-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<long> ReadInt64Async(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt64(address), cancellationToken);
            return reply.GetPayloadInt64();
        }

        /// <summary>
        /// Asynchronously reads the value of a 64-bit signed integer array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<long[]> ReadInt64ArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadInt64(address), cancellationToken);
            return reply.GetPayloadArray<long>();
        }

        /// <summary>
        /// Asynchronously reads the value of a single-precision floating point register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the value of the register.
        /// </returns>
        public async Task<float> ReadSingleAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(address), cancellationToken);
            return reply.GetPayloadSingle();
        }

        /// <summary>
        /// Asynchronously reads the value of a single-precision floating point array register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the state of the array register.
        /// </returns>
        public async Task<float[]> ReadSingleArrayAsync(int address, CancellationToken cancellationToken = default)
        {
            var reply = await CommandAsync(HarpCommand.ReadSingle(address), cancellationToken);
            return reply.GetPayloadArray<float>();
        }

        /// <summary>
        /// Asynchronously updates the integral part of the system timestamp, in seconds.
        /// </summary>
        /// <param name="seconds">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteTimestampSecondsAsync(uint seconds, CancellationToken cancellationToken = default)
        {
            await WriteUInt32Async(TimestampSeconds.Address, seconds, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends a command to reset the device and restore or
        /// save non-volatile registers.
        /// </summary>
        /// <param name="reset">
        /// A value specifying whether to restore or save non-volatile registers.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous reset operation.
        /// </returns>
        public async Task WriteResetDeviceAsync(ResetFlags reset, CancellationToken cancellationToken = default)
        {
            await WriteByteAsync(ResetDevice.Address, (byte)reset, cancellationToken);
        }

        /// <summary>
        /// Asynchronously updates the display name of the device.
        /// </summary>
        /// <param name="name">
        /// A <see cref="string"/> containing the name of the device. The maximum length
        /// of the specified device name is 25 characters.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteDeviceNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The specified device name cannot be null or empty.", nameof(name));
            }

            await CommandAsync(DeviceName.FromPayload(MessageType.Write, name), cancellationToken);
        }

        /// <summary>
        /// Asynchronously updates the configuration for the device synchronization clock.
        /// </summary>
        /// <param name="value">A value specifying configuration flags for the device synchronization clock.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>The task object representing the asynchronous write operation.</returns>
        public async Task WriteClockConfigurationAsync(ClockConfigurationFlags value, CancellationToken cancellationToken = default)
        {
            await WriteByteAsync(ClockConfiguration.Address, (byte)value, cancellationToken);
        }

        /// <summary>
        /// Asynchronously writes a value to an 8-bit unsigned integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteByteAsync(int address, byte value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteByte(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to an 8-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteByteAsync(int address, byte[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteByte(address, values), cancellationToken);

        /// <summary>
        /// Asynchronously writes a value to an 8-bit signed integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteSByteAsync(int address, sbyte value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteSByte(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to an 8-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteSByteAsync(int address, sbyte[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteSByte(address, values), cancellationToken);

        /// <summary>
        /// Asynchronously writes a value to a 16-bit unsigned integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt16Async(int address, ushort value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteUInt16(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to a 16-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt16Async(int address, ushort[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteUInt16(address, values), cancellationToken);

        /// <summary>
        /// Asynchronously writes a value to a 16-bit signed integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt16Async(int address, short value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteInt16(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to a 16-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt16Async(int address, short[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteInt16(address, values), cancellationToken);

        /// <summary>
        /// Asynchronously writes a value to a 32-bit unsigned integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt32Async(int address, uint value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteUInt32(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to a 32-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt32Async(int address, uint[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteUInt32(address, values), cancellationToken);

        /// <summary>
        /// Asynchronously writes a value to a 32-bit signed integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt32Async(int address, int value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteInt32(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to a 32-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt32Async(int address, int[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteInt32(address, values), cancellationToken);

        /// <summary>
        /// Asynchronously writes a value to a 64-bit unsigned integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt64Async(int address, ulong value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteUInt64(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to a 64-bit unsigned integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteUInt64Async(int address, ulong[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteUInt64(address, values), cancellationToken);

        /// <summary>
        /// Asynchronously writes a value to a 64-bit signed integer register
        /// with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt64Async(int address, long value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteInt64(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to a 64-bit signed integer register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteInt64Async(int address, long[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteInt64(address, values), cancellationToken);

        /// <summary>
        /// Asynchronously writes a value to a single-precision floating point register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteSingleAsync(int address, float value, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteSingle(address, value), cancellationToken);

        /// <summary>
        /// Asynchronously writes an array of values to a single-precision floating point register with
        /// the specified address.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="values">The values to be stored in the register.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous write operation.
        /// </returns>
        public async Task WriteSingleAsync(int address, float[] values, CancellationToken cancellationToken = default)
            => await CommandAsync(HarpCommand.WriteSingle(address, values), cancellationToken);

        /// <summary>
        /// Sends a command to the Harp device and awaits the response as an asynchronous operation.
        /// </summary>
        /// <param name="command">The <see cref="HarpMessage"/> specifying the command to send.</param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation. The <see cref="Task{TResult}.Result"/>
        /// property contains the message representing the response of the device to the asynchronous command.
        /// </returns>
        public async Task<HarpMessage> CommandAsync(HarpMessage command, CancellationToken cancellationToken = default)
        {
            var reply = response.FirstAsync(message =>
            {
                var match = message.IsMatch(command.Address, command.MessageType);
                if (match && message.Error)
                {
                    throw new HarpException(message);
                }

                return match;
            }).RunAsync(cancellationToken);

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

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (!_leaveOpen)
            {
                await transport.CloseAsync();
            }
            response.Dispose();
        }
    }
}

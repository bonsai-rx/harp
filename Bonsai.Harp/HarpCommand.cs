using System;
using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides static methods for creating Harp command messages.
    /// </summary>
    public static class HarpCommand
    {
        #region Common

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> command to initialize the device operation control register.
        /// </summary>
        /// <param name="operationMode">The desired operation mode of the device.</param>
        /// <param name="ledState">Specifies whether the operation mode LED should report the device state.</param>
        /// <param name="visualIndicators">Specifies whether any visual indicator LEDs should be enabled on the Harp device.</param>
        /// <param name="heartbeat">Specifies whether to enable or disable the heartbeat register.</param>
        /// <param name="replies">Specifies whether write commands should report back the state of the register.</param>
        /// <param name="dumpRegisters">Specifies whether the state of all registers should be reported after initialization.</param>
        /// <returns>A valid <see cref="HarpMessage"/> command to set the device operation mode.</returns>
        public static HarpMessage OperationControl(DeviceState operationMode, LedState ledState, LedState visualIndicators, EnableType heartbeat, EnableType replies, bool dumpRegisters)
        {
            int operationFlags;
            operationFlags  = heartbeat == EnableType.Enable      ? 0x80 : 0x00;
            operationFlags += ledState == LedState.On             ? 0x40 : 0x00;
            operationFlags += visualIndicators == LedState.On     ? 0x20 : 0x00;
            operationFlags += replies == EnableType.Enable        ? 0x00 : 0x10;
            operationFlags += dumpRegisters                       ? 0x08 : 0x00;
            operationFlags += operationMode == DeviceState.Active ? 0x01 : 0x00;
            return WriteByte(DeviceRegisters.OperationControl, (byte)operationFlags);
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static HarpMessage Reset(ResetMode resetMode)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return ResetDevice(resetMode);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> command to reset the device and restore or save non-volatile registers.
        /// </summary>
        /// <param name="resetMode">Specifies whether to restore or save non-volatile registers.</param>
        /// <returns>A valid <see cref="HarpMessage"/> command to reset the device.</returns>
        public static HarpMessage ResetDevice(ResetMode resetMode)
        {
            return WriteByte(DeviceRegisters.ResetDevice, (byte)(1 << (byte)resetMode));
        }

        #endregion

        #region Read

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for the specified address and payload type.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <param name="payloadType">The type of data available in the register.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command with the specified address and payload type.
        /// </returns>
        public static HarpMessage Read(int address, PayloadType payloadType)
        {
            return HarpMessage.FromPayload(address, MessageType.Read, payloadType);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for an 8-bit unsigned integer
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for an 8-bit unsigned integer
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadByte(int address)
        {
            return HarpMessage.FromByte(address, MessageType.Read);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for an 8-bit signed integer
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for an 8-bit signed integer
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadSByte(int address)
        {
            return HarpMessage.FromSByte(address, MessageType.Read);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for a 16-bit unsigned integer
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for a 16-bit unsigned integer
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadUInt16(int address)
        {
            return HarpMessage.FromUInt16(address, MessageType.Read);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for a 16-bit signed integer
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for a 16-bit signed integer
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadInt16(int address)
        {
            return HarpMessage.FromInt16(address, MessageType.Read);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for a 32-bit unsigned integer
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for a 32-bit unsigned integer
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadUInt32(int address)
        {
            return HarpMessage.FromUInt32(address, MessageType.Read);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for a 32-bit signed integer
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for a 32-bit signed integer
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadInt32(int address)
        {
            return HarpMessage.FromInt32(address, MessageType.Read);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for a 64-bit unsigned integer
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for a 64-bit unsigned integer
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadUInt64(int address)
        {
            return HarpMessage.FromUInt64(address, MessageType.Read);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for a 64-bit signed integer
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for a 64-bit signed integer
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadInt64(int address)
        {
            return HarpMessage.FromInt64(address, MessageType.Read);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> read command for a single-precision floating point
        /// register with the specified address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> read command for a single-precision floating point
        /// register with the specified address.
        /// </returns>
        public static HarpMessage ReadSingle(int address)
        {
            return HarpMessage.FromSingle(address, MessageType.Read);
        }

        #endregion

        #region Write

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="payloadType">The type of data available in the message payload.</param>
        /// <param name="payload">The raw binary representation of the payload data.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage Write(int address, PayloadType payloadType, params byte[] payload)
        {
            return HarpMessage.FromPayload(address, MessageType.Write, payloadType, payload);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and a
        /// single value 8-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteByte(int address, byte value)
        {
            return HarpMessage.FromByte(address, MessageType.Write, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of 8-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteByte(int address, params byte[] values)
        {
            return HarpMessage.FromByte(address, MessageType.Write, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and a
        /// single value 8-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteSByte(int address, sbyte value)
        {
            return HarpMessage.FromSByte(address, MessageType.Write, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of 8-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteSByte(int address, params sbyte[] values)
        {
            return HarpMessage.FromSByte(address, MessageType.Write, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and a
        /// single value 16-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteUInt16(int address, ushort value)
        {
            return HarpMessage.FromUInt16(address, MessageType.Write, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of 16-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteUInt16(int address, params ushort[] values)
        {
            return HarpMessage.FromUInt16(address, MessageType.Write, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and a
        /// single value 16-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteInt16(int address, short value)
        {
            return HarpMessage.FromInt16(address, MessageType.Write, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of 16-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteInt16(int address, params short[] values)
        {
            return HarpMessage.FromInt16(address, MessageType.Write, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and a
        /// single value 32-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteUInt32(int address, uint value)
        {
            return HarpMessage.FromUInt32(address, MessageType.Write, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of 32-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteUInt32(int address, params uint[] values)
        {
            return HarpMessage.FromUInt32(address, MessageType.Write, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and a
        /// single value 32-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteInt32(int address, int value)
        {
            return HarpMessage.FromInt32(address, MessageType.Write, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of 32-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteInt32(int address, params int[] values)
        {
            return HarpMessage.FromInt32(address, MessageType.Write, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and a
        /// single value 64-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteUInt64(int address, ulong value)
        {
            return HarpMessage.FromUInt64(address, MessageType.Write, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of 64-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteUInt64(int address, params ulong[] values)
        {
            return HarpMessage.FromUInt64(address, MessageType.Write, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and a
        /// single value 64-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteInt64(int address, long value)
        {
            return HarpMessage.FromInt64(address, MessageType.Write, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of 64-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteInt64(int address, params long[] values)
        {
            return HarpMessage.FromInt64(address, MessageType.Write, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, and an
        /// array payload of single-precision floating point numbers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address and payload.
        /// </returns>
        public static HarpMessage WriteSingle(int address, params float[] values)
        {
            return HarpMessage.FromSingle(address, MessageType.Write, values);
        }

        #endregion
    }
}

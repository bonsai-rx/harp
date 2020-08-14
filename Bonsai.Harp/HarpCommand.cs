﻿namespace Bonsai.Harp
{
    /// <summary>
    /// Provides static methods for creating Harp command messages.
    /// </summary>
    public static class HarpCommand
    {
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="payloadType">The type of data available in the message payload.</param>
        /// <param name="payload">The raw binary representation of the payload data.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage Write(int address, double timestamp, PayloadType payloadType, params byte[] payload)
        {
            return HarpMessage.FromPayload(address, timestamp, MessageType.Write, payloadType, payload);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and a
        /// single value 8-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteByte(int address, double timestamp, byte value)
        {
            return HarpMessage.FromByte(address, timestamp, MessageType.Write, value);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of 8-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteByte(int address, double timestamp, params byte[] values)
        {
            return HarpMessage.FromByte(address, timestamp, MessageType.Write, values);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and a
        /// single value 8-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteSByte(int address, double timestamp, sbyte value)
        {
            return HarpMessage.FromSByte(address, timestamp, MessageType.Write, value);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of 8-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteSByte(int address, double timestamp, params sbyte[] values)
        {
            return HarpMessage.FromSByte(address, timestamp, MessageType.Write, values);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and a
        /// single value 16-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteUInt16(int address, double timestamp, ushort value)
        {
            return HarpMessage.FromUInt16(address, timestamp, MessageType.Write, value);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of 16-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteUInt16(int address, double timestamp, params ushort[] values)
        {
            return HarpMessage.FromUInt16(address, timestamp, MessageType.Write, values);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and a
        /// single value 16-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteInt16(int address, double timestamp, short value)
        {
            return HarpMessage.FromInt16(address, timestamp, MessageType.Write, value);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of 16-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteInt16(int address, double timestamp, params short[] values)
        {
            return HarpMessage.FromInt16(address, timestamp, MessageType.Write, values);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and a
        /// single value 32-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteUInt32(int address, double timestamp, uint value)
        {
            return HarpMessage.FromUInt32(address, timestamp, MessageType.Write, value);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of 32-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteUInt32(int address, double timestamp, params uint[] values)
        {
            return HarpMessage.FromUInt32(address, timestamp, MessageType.Write, values);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and a
        /// single value 32-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteInt32(int address, double timestamp, int value)
        {
            return HarpMessage.FromInt32(address, timestamp, MessageType.Write, value);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of 32-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteInt32(int address, double timestamp, params int[] values)
        {
            return HarpMessage.FromInt32(address, timestamp, MessageType.Write, values);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and a
        /// single value 64-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteUInt64(int address, double timestamp, ulong value)
        {
            return HarpMessage.FromUInt64(address, timestamp, MessageType.Write, value);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of 64-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteUInt64(int address, double timestamp, params ulong[] values)
        {
            return HarpMessage.FromUInt64(address, timestamp, MessageType.Write, values);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and a
        /// single value 64-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteInt64(int address, double timestamp, long value)
        {
            return HarpMessage.FromInt64(address, timestamp, MessageType.Write, value);
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
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of 64-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteInt64(int address, double timestamp, params long[] values)
        {
            return HarpMessage.FromInt64(address, timestamp, MessageType.Write, values);
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> write command with the specified address, timestamp, and an
        /// array payload of single-precision floating point numbers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> write command with the specified address, timestamp, and payload.
        /// </returns>
        public static HarpMessage WriteSingle(int address, double timestamp, params float[] values)
        {
            return HarpMessage.FromSingle(address, timestamp, MessageType.Write, values);
        }

        #endregion
    }
}

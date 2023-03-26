namespace Bonsai.Harp
{
    /// <summary>
    /// Specifies the type of a Harp device message.
    /// </summary>
    public enum MessageType : byte
    {
        /// <summary>
        /// The device is reading the contents of the register at the specified address.
        /// </summary>
        Read = 0x01,

        /// <summary>
        /// The device is writing the message payload to the register at the specified address.
        /// </summary>
        Write = 0x02,

        /// <summary>
        /// The device is reporting the contents of the register at the specified address.
        /// </summary>
        Event = 0x03
    }
}

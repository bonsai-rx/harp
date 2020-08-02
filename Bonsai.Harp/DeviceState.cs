namespace Bonsai.Harp
{
    /// <summary>
    /// Specifies the desired state of the device after initialization.
    /// </summary>
    public enum DeviceState : byte
    {
        /// <summary>
        /// Specifies that the device should be active and acquiring data immediately following initialization.
        /// </summary>
        Active = 0,

        /// <summary>
        /// Specifies that the device should be on standby without acquiring data immediately following initialization.
        /// </summary>
        Standby = 1
    }
}

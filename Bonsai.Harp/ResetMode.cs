namespace Bonsai.Harp
{
    /// <summary>
    /// Specifies the behavior of the non-volatile registers when resetting the device.
    /// </summary>
    public enum ResetMode : byte
    {
        /// <summary>
        /// The device will boot with all the registers reset to their default factory values.
        /// </summary>
        RestoreDefault,

        /// <summary>
        /// The device will boot and restore all the registers to the values stored in non-volatile memory.
        /// </summary>
        RestoreEeprom,

        /// <summary>
        /// The device will boot and save all the current register values to non-volatile memory.
        /// </summary>
        Save,

        /// <summary>
        /// The device will boot with the default device name.
        /// </summary>
        RestoreName
    }
}

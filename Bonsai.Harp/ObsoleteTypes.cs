using System;

namespace Bonsai.Harp
{
    /// <summary>
    /// Specifies the desired state of the device after initialization.
    /// </summary>
    [Obsolete]
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

    /// <summary>
    /// Specifies whether a specific register is enabled or disabled.
    /// </summary>
    [Obsolete]
    public enum EnableType : byte
    {
        /// <summary>
        /// Specifies that the register is enabled.
        /// </summary>
        Enable = 0,

        /// <summary>
        /// Specifies that the register is disabled.
        /// </summary>
        Disable = 1
    }

    /// <summary>
    /// Specifies the behavior of the non-volatile registers when resetting the device.
    /// </summary>
    [Obsolete]
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

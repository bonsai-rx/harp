namespace Bonsai.Harp
{
    /// <summary>
    /// Specifies whether a specific register is enabled or disabled.
    /// </summary>
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
}

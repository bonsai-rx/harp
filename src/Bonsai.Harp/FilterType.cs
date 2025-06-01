namespace Bonsai.Harp
{
    /// <summary>
    /// Specifies how the message filter will use the matching criteria.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Specifies the filter should accept all messages matching the criteria.
        /// </summary>
        Include,

        /// <summary>
        /// Specifies the filter should reject all messages matching the criteria.
        /// </summary>
        Exclude
    }
}

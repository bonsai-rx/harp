using System;
using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which filters a sequence of Harp messages for elements that match the specified address and message type.
    /// </summary>
    [Obsolete]
    [WorkflowElementCategory(ElementCategory.Condition)]
    [Description("Filters a sequence of Harp messages for elements that match the specified address and message type.")]
    public class FilterHarpMessage : FilterMessage
    {
    }
}

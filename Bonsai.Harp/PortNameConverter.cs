using System.ComponentModel;
using System.IO.Ports;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides a type converter to convert port name strings to and from
    /// other representations.
    /// </summary>
    public class PortNameConverter : StringConverter
    {
        /// <inheritdoc/>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <inheritdoc/>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(SerialPort.GetPortNames());
        }
    }
}

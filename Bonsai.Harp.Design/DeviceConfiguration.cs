using System;
using System.ComponentModel;
using System.Globalization;

namespace Bonsai.Harp.Design
{
    class DeviceConfiguration
    {
        [Browsable(false)]
        public string Id
        {
            get { return !SerialNumber.HasValue ? $"{WhoAmI}" : $"{WhoAmI}-{SerialNumber:x4}"; }
            set
            {
                var parts = value?.Split('-');
                if (parts?.Length <= 2)
                {
                    throw new ArgumentException("The id string is null or has an invalid format.", nameof(value));
                }

                WhoAmI = int.Parse(parts[0]);
                if (parts.Length == 2)
                {
                    SerialNumber = int.Parse(parts[1], NumberStyles.HexNumber);
                }
            }
        }

        public string DeviceName { get; set; }

        internal int FirmwareVersionHigh { get; set; }

        internal int FirmwareVersionLow { get; set; }

        internal int CoreVersionHigh { get; set; }

        internal int CoreVersionLow { get; set; }

        internal int HardwareVersionHigh { get; set; }

        internal int HardwareVersionLow { get; set; }

        public HarpVersion FirmwareVersion
        {
            get { return new HarpVersion(FirmwareVersionHigh, FirmwareVersionLow); }
        }

        public HarpVersion CoreVersion
        {
            get { return new HarpVersion(CoreVersionHigh, CoreVersionLow); }
        }

        public HarpVersion HardwareVersion
        {
            get { return new HarpVersion(HardwareVersionHigh, HardwareVersionLow); }
        }

        public int AssemblyVersion { get; internal set; }

        public int WhoAmI { get; internal set; }

        public int? SerialNumber { get; internal set; }

        [DisplayName("Timestamp (s)")]
        public uint Timestamp { get; internal set; }

        public override string ToString()
        {
            return string.Join(
                Environment.NewLine,
                !SerialNumber.HasValue ? $"WhoAmI: {WhoAmI}" : $"WhoAmI: {WhoAmI}-{SerialNumber:x4}",
                $"HardwareVersion: {HardwareVersionHigh}.{HardwareVersionLow}",
                $"FirmwareVersion: {FirmwareVersionHigh}.{FirmwareVersionLow}",
                $"CoreVersion: {CoreVersionHigh}.{CoreVersionLow}",
                $"Timestamp (s): {Timestamp}",
                $"DeviceName: {DeviceName}");
        }
    }
}

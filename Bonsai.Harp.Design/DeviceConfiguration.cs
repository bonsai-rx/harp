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
            get { return $"{WhoAmI}-{SerialNumber:x4}"; }
            set
            {
                var parts = value?.Split('-');
                if (parts?.Length != 2)
                {
                    throw new ArgumentException("The id string is empty or has an invalid format.", nameof(value));
                }

                WhoAmI = int.Parse(parts[0]);
                SerialNumber = int.Parse(parts[1], NumberStyles.HexNumber);
            }
        }

        internal int WhoAmI { get; set; }

        public string DeviceName { get; internal set; }

        internal int FirmwareVersionHigh { get; set; }

        internal int FirmwareVersionLow { get; set; }

        internal int HardwareVersionHigh { get; set; }

        internal int HardwareVersionLow { get; set; }

        public HarpVersion FirmwareVersion
        {
            get { return new HarpVersion(FirmwareVersionHigh, FirmwareVersionLow); }
        }

        public HarpVersion HardwareVersion
        {
            get { return new HarpVersion(HardwareVersionHigh, HardwareVersionLow); }
        }

        internal int? SerialNumber { get; set; }

        [DisplayName("Timestamp (s)")]
        public uint Timestamp { get; internal set; }

        public FirmwareMetadata GetFirmwareMetadata()
        {
            var protocolVersion = new HarpVersion(1, null);
            return new FirmwareMetadata(DeviceName, FirmwareVersion, protocolVersion, HardwareVersion);
        }

        public override string ToString()
        {
            return string.Join(
                Environment.NewLine,
                !SerialNumber.HasValue ? $"WhoAmI: {WhoAmI}" : $"WhoAmI: {WhoAmI}-{SerialNumber:x4}",
                $"HardwareVersion: {HardwareVersionHigh}.{HardwareVersionLow}",
                $"FirmwareVersion: {FirmwareVersionHigh}.{FirmwareVersionLow}",
                $"Timestamp (s): {Timestamp}",
                $"DeviceName: {DeviceName}");
        }
    }
}

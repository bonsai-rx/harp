using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents information about the device, firmware version and hardware version numbers
    /// contained in a particular device or hex file.
    /// </summary>
    public sealed class FirmwareMetadata : IEquatable<FirmwareMetadata>
    {
        static readonly Regex MetadataRegex = new Regex("^(?<device>\\w+)-fw(?<firmware>\\d+\\.\\d+)-harp(?<protocol>\\d+\\.\\d+)-hw(?<hardware>(?:x|\\d+)\\.(?:x|\\d+))-ass(?<sequence>x|\\d+)(?:-preview(?<prerelease>\\d+))?$");

        /// <summary>
        /// Initializes a new instance of the <see cref="FirmwareMetadata"/> class with the
        /// specified device name, the firmware version and compatible hardware versions.
        /// </summary>
        /// <param name="deviceName">The unique identifier of the device type on which the firmware should be installed.</param>
        /// <param name="firmwareVersion">The version of the firmware contained in the device or hex file.</param>
        /// <param name="protocolVersion">The version of the Harp protocol implemented by the firmware.</param>
        /// <param name="hardwareVersion">The hardware version of the device, or range of hardware versions supported by the firmware.</param>
        /// <param name="assemblyNumber">The board assembly version of the device, or range of assembly versions supported by the firmware.</param>
        /// <param name="prereleaseVersion">The optional prerelease number, for preview versions of the firmware.</param>
        public FirmwareMetadata(
            string deviceName,
            HarpVersion firmwareVersion,
            HarpVersion protocolVersion,
            HarpVersion hardwareVersion,
            int? assemblyNumber = default,
            int? prereleaseVersion = default)
        {
            DeviceName = deviceName ?? throw new ArgumentNullException(nameof(deviceName));
            FirmwareVersion = firmwareVersion ?? throw new ArgumentNullException(nameof(firmwareVersion));
            ProtocolVersion = protocolVersion ?? throw new ArgumentNullException(nameof(protocolVersion));
            HardwareVersion = hardwareVersion ?? throw new ArgumentNullException(nameof(hardwareVersion));
            AssemblyNumber = assemblyNumber;
            PrereleaseVersion = prereleaseVersion;
        }

        /// <summary>
        /// Gets the unique identifier of the device type on which the firmware should be installed.
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        /// Gets the version of the firmware contained in the device or hex file.
        /// </summary>
        public HarpVersion FirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the version of the Harp protocol implemented by the firmware.
        /// </summary>
        public HarpVersion ProtocolVersion { get; private set; }

        /// <summary>
        /// Gets the hardware version of the device, or range of hardware versions supported by the firmware.
        /// </summary>
        public HarpVersion HardwareVersion { get; private set; }

        /// <summary>
        /// Gets the board assembly version of the device, or range of assembly versions supported by the firmware.
        /// </summary>
        public int? AssemblyNumber { get; private set; }

        /// <summary>
        /// Gets the optional prerelease number, for preview versions of the firmware.
        /// </summary>
        public int? PrereleaseVersion { get; private set; }

        /// <summary>
        /// Returns whether the firmware supports the specified hardware version
        /// and board assembly number.
        /// </summary>
        /// <param name="deviceName">The identifier of the device to check for compatibility.</param>
        /// <param name="hardwareVersion">The hardware version to check for compatibility.</param>
        /// <param name="assemblyNumber">The optional board assembly number to check for compatibility.</param>
        /// <returns>
        /// <b>true</b> if the firmware supports the specified <paramref name="hardwareVersion"/> and
        /// <paramref name="assemblyNumber"/>; otherwise, <b>false</b>.
        /// </returns>
        public bool Supports(string deviceName, HarpVersion hardwareVersion, int assemblyNumber = default)
        {
            return DeviceName == deviceName &&
                   HardwareVersion.Satisfies(hardwareVersion) &&
                   (!AssemblyNumber.HasValue || AssemblyNumber.Value == assemblyNumber);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current metadata.
        /// </summary>
        /// <param name="obj">The object to compare with the current metadata.</param>
        /// <returns>
        /// <b>true</b> if the specified object is equal to the current metadata;
        /// otherwise, <b>false</b>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is FirmwareMetadata version) return Equals(version);
            else return false;
        }

        /// <summary>
        /// Determines whether the specified metadata object is equal to the current metadata.
        /// </summary>
        /// <param name="other">The metadata object to compare with the current metadata.</param>
        /// <returns>
        /// <b>true</b> if the specified metadata object is equal to the current metadata;
        /// otherwise, <b>false</b>.
        /// </returns>
        public bool Equals(FirmwareMetadata other)
        {
            if (other is null) return false;
            return DeviceName == other.DeviceName &&
                   FirmwareVersion.Equals(other.FirmwareVersion) &&
                   ProtocolVersion.Equals(other.ProtocolVersion) &&
                   HardwareVersion.Equals(other.HardwareVersion) &&
                   AssemblyNumber == other.AssemblyNumber &&
                   PrereleaseVersion == other.PrereleaseVersion;
        }

        /// <summary>
        /// Computes the hash code for the current metadata object.
        /// </summary>
        /// <returns>
        /// The hash code for the current metadata object, extracted from a combination
        /// of hashes for the device name and various version numbers.
        /// </returns>
        public override int GetHashCode()
        {
            return 17 * DeviceName.GetHashCode() +
                   8971 * FirmwareVersion.GetHashCode() +
                   2803 * ProtocolVersion.GetHashCode() +
                   691 * HardwareVersion.GetHashCode() +
                   1409 * AssemblyNumber.GetHashCode() +
                   2333 * PrereleaseVersion.GetHashCode();
        }

        /// <summary>
        /// Determines whether the values on both sides of the equality operator
        /// are equal.
        /// </summary>
        /// <param name="lhs">The value on the left-hand side of the operator.</param>
        /// <param name="rhs">The value on the right-hand side of the operator.</param>
        /// <returns>
        /// <b>true</b> if the value on the left-hand side of the operator is equal
        /// to the value on the right-hand side; otherwise, <b>false</b>.
        /// </returns>
        public static bool operator ==(FirmwareMetadata lhs, FirmwareMetadata rhs)
        {
            if (lhs is null) return rhs is null;
            else return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether the values on both sides of the inequality operator
        /// are not equal.
        /// </summary>
        /// <param name="lhs">The value on the left-hand side of the operator.</param>
        /// <param name="rhs">The value on the right-hand side of the operator.</param>
        /// <returns>
        /// <b>true</b> if the value on the left-hand side of the operator is not equal
        /// to the value on the right-hand side; otherwise, <b>false</b>.
        /// </returns>
        public static bool operator !=(FirmwareMetadata lhs, FirmwareMetadata rhs)
        {
            if (lhs is null) return !(rhs is null);
            else return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Converts a string representation of the <see cref="FirmwareMetadata"/> to its
        /// equivalent value.
        /// </summary>
        /// <param name="input">The string representing the <see cref="FirmwareMetadata"/>.</param>
        /// <returns>The equivalent <see cref="FirmwareMetadata"/> object for the specified string representation.</returns>
        public static FirmwareMetadata Parse(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (!TryParse(input, out FirmwareMetadata result))
            {
                throw new ArgumentException("Invalid Harp firmware metadata specification string.", nameof(input));
            }

            return result;
        }

        /// <summary>
        /// Converts a string representation of the <see cref="FirmwareMetadata"/> to its
        /// equivalent value. A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="input">The string representing the <see cref="FirmwareMetadata"/>.</param>
        /// <param name="metadata">
        /// When this method returns, contains the equivalent <see cref="FirmwareMetadata"/> object
        /// for the specified string representation if the conversion was successful;
        /// otherwise, contains <b>null</b>.
        /// </param>
        /// <returns><b>true</b> if the conversion was successful; otherwise, <b>false</b>.</returns>
        public static bool TryParse(string input, out FirmwareMetadata metadata)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var match = MetadataRegex.Match(input);
            if (match.Success && match.Groups.Count == 7)
            {
                var deviceName = match.Groups[1].Value;
                var firmwareVersion = HarpVersion.Parse(match.Groups[2].Value);
                var protocolVersion = HarpVersion.Parse(match.Groups[3].Value);
                var hardwareVersion = HarpVersion.Parse(match.Groups[4].Value);
                var assemblyNumber = match.Groups[5].Value == HarpVersion.FloatingWildcard ? (int?)null : int.Parse(match.Groups[5].Value);
                var prereleaseVersion = string.IsNullOrEmpty(match.Groups[6].Value) ? (int?)null : int.Parse(match.Groups[6].Value);
                metadata = new FirmwareMetadata(deviceName, firmwareVersion, protocolVersion, hardwareVersion, assemblyNumber, prereleaseVersion);
                return true;
            }
            else
            {
                metadata = null;
                return false;
            }
        }

        /// <summary>
        /// Converts the <see cref="FirmwareMetadata"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>
        /// The string representation of the <see cref="FirmwareMetadata"/> object.
        /// </returns>
        public override string ToString()
        {
            var prerelease = PrereleaseVersion.HasValue ? $"-preview{PrereleaseVersion.Value}" : string.Empty;
            var assemblyNumber = AssemblyNumber.HasValue ? AssemblyNumber.Value.ToString(CultureInfo.InvariantCulture) : HarpVersion.FloatingWildcard;
            return $"{DeviceName}-fw{FirmwareVersion}-harp{ProtocolVersion}-hw{HardwareVersion}-ass{assemblyNumber}{prerelease}";
        }
    }
}

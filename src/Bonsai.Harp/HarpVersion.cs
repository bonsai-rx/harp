﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents the major and minor version of Harp firmware or hardware.
    /// </summary>
    public sealed class HarpVersion : IComparable, IComparable<HarpVersion>, IEquatable<HarpVersion>
    {
        internal const string FloatingWildcard = "x";
        static readonly Regex VersionRegex = new Regex("^(?<major>x|\\d+)\\.(?<minor>x|\\d+)$");

        /// <summary>
        /// Initializes a new instance of the <see cref="HarpVersion"/> class with the specified
        /// major and minor version.
        /// </summary>
        /// <param name="major">The optional major version. If not specified, matches against all versions.</param>
        /// <param name="minor">
        /// The optional minor version. If not specified, matches against all minor versions with the same major version.
        /// </param>
        public HarpVersion(int? major, int? minor)
        {
            if (major == null && minor != null)
            {
                throw new ArgumentException("Minor version cannot be specified if major version is floating.", nameof(minor));
            }

            Major = major;
            Minor = minor;
        }

        /// <summary>
        /// Gets the optional major version.
        /// </summary>
        public int? Major { get; private set; }

        /// <summary>
        /// Gets the optional minor version.
        /// </summary>
        public int? Minor { get; private set; }

        /// <summary>
        /// Returns whether the specified version matches the current version, taking into account
        /// compatible floating ranges.
        /// </summary>
        /// <param name="other">The <see cref="HarpVersion"/> with which to compare.</param>
        /// <returns>
        /// <b>true</b> if <paramref name="other"/> matches against the current version;
        /// otherwise, <b>false</b>.
        /// </returns>
        public bool Satisfies(HarpVersion other)
        {
            if (other is null) return false;

            var satisfyMajor = !Major.HasValue || Major == other.Major.GetValueOrDefault(Major.Value);
            if (!satisfyMajor) return false;

            return !Minor.HasValue || Minor == other.Minor.GetValueOrDefault(Minor.Value);
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is HarpVersion version) return CompareTo(version);
            else throw new ArgumentException($"Object is not a {typeof(HarpVersion)}.", nameof(obj));
        }

        /// <summary>
        /// Performs a comparison with another version object and returns a value indicating
        /// whether this version is less than, equal, or greater than the other.
        /// </summary>
        /// <param name="other">The version object to compare with.</param>
        /// <returns>
        /// A negative number if this version is lower than the other version; zero if it
        /// is the same version; a positive number if this version is higher than the other
        /// version. Floating wildcards are always smaller for the purposes of ordering.
        /// </returns>
        public int CompareTo(HarpVersion other)
        {
            if (other is null) return 1;

            var major = Comparer<int?>.Default.Compare(Major, other.Major);
            if (major != 0) return major;

            return Comparer<int?>.Default.Compare(Minor, other.Minor);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current version.
        /// </summary>
        /// <param name="obj">The object to compare with the current version.</param>
        /// <returns>
        /// <b>true</b> if the specified object is equal to the current version;
        /// otherwise, <b>false</b>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is HarpVersion version) return Equals(version);
            else return false;
        }

        /// <summary>
        /// Determines whether the specified version is equal to the current version.
        /// </summary>
        /// <param name="other">The version object to compare with the current version.</param>
        /// <returns>
        /// <b>true</b> if the specified version object is equal to the current version;
        /// otherwise, <b>false</b>.
        /// </returns>
        public bool Equals(HarpVersion other)
        {
            if (other is null) return false;
            return Major == other.Major && Minor == other.Minor;
        }

        /// <summary>
        /// Computes the hash code for the current version object.
        /// </summary>
        /// <returns>
        /// The hash code for the current version object, extracted from a
        /// combination of the major and minor version hashes.
        /// </returns>
        public override int GetHashCode()
        {
            return 29863 * Major.GetHashCode() + 1723 * Minor.GetHashCode();
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
        public static bool operator ==(HarpVersion lhs, HarpVersion rhs)
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
        public static bool operator !=(HarpVersion lhs, HarpVersion rhs)
        {
            if (lhs is null) return !(rhs is null);
            else return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether the value on the left-hand side of the operator is less than
        /// the value on the right-hand side.
        /// </summary>
        /// <param name="lhs">The value on the left-hand side of the operator.</param>
        /// <param name="rhs">The value on the right-hand side of the operator.</param>
        /// <returns>
        /// <b>true</b> if the value on the left-hand side of the operator is less than
        /// the value on the right-hand side; otherwise, <b>false</b>.
        /// </returns>
        public static bool operator <(HarpVersion lhs, HarpVersion rhs)
        {
            if (lhs is null) return !(rhs is null);
            else return lhs.CompareTo(rhs) < 0;
        }

        /// <summary>
        /// Determines whether the value on the left-hand side of the operator is greater than
        /// the value on the right-hand side.
        /// </summary>
        /// <param name="lhs">The value on the left-hand side of the operator.</param>
        /// <param name="rhs">The value on the right-hand side of the operator.</param>
        /// <returns>
        /// <b>true</b> if the value on the left-hand side of the operator is greater than
        /// the value on the right-hand side; otherwise, <b>false</b>.
        /// </returns>
        public static bool operator >(HarpVersion lhs, HarpVersion rhs)
        {
            if (lhs is null) return false;
            else return lhs.CompareTo(rhs) > 0;
        }

        /// <summary>
        /// Determines whether the value on the left-hand side of the operator is less than
        /// or equal to the value on the right-hand side.
        /// </summary>
        /// <param name="lhs">The value on the left-hand side of the operator.</param>
        /// <param name="rhs">The value on the right-hand side of the operator.</param>
        /// <returns>
        /// <b>true</b> if the value on the left-hand side of the operator is less than
        /// or equal to the value on the right-hand side; otherwise, <b>false</b>.
        /// </returns>
        public static bool operator <=(HarpVersion lhs, HarpVersion rhs)
        {
            if (lhs is null) return true;
            else return lhs.CompareTo(rhs) <= 0;
        }

        /// <summary>
        /// Determines whether the value on the left-hand side of the operator is greater than
        /// or equal to the value on the right-hand side.
        /// </summary>
        /// <param name="lhs">The value on the left-hand side of the operator.</param>
        /// <param name="rhs">The value on the right-hand side of the operator.</param>
        /// <returns>
        /// <b>true</b> if the value on the left-hand side of the operator is greater than
        /// or equal to the value on the right-hand side; otherwise, <b>false</b>.
        /// </returns>
        public static bool operator >=(HarpVersion lhs, HarpVersion rhs)
        {
            if (lhs is null) return rhs is null;
            else return lhs.CompareTo(rhs) >= 0;
        }

        /// <summary>
        /// Converts a string representation of a <see cref="HarpVersion"/> to its
        /// equivalent value.
        /// </summary>
        /// <param name="version">The string representing a <see cref="HarpVersion"/>.</param>
        /// <returns>The equivalent <see cref="HarpVersion"/> object for the specified string representation.</returns>
        public static HarpVersion Parse(string version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (!TryParse(version, out HarpVersion result))
            {
                throw new ArgumentException("Invalid Harp version specification string.", nameof(version));
            }

            return result;
        }

        /// <summary>
        /// Converts a string representation of a <see cref="HarpVersion"/> to its
        /// equivalent value. A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="version">The string representing a <see cref="HarpVersion"/>.</param>
        /// <param name="value">
        /// When this method returns, contains the equivalent <see cref="HarpVersion"/> object
        /// for the specified string representation if the conversion was successful;
        /// otherwise, contains <b>null</b>.
        /// </param>
        /// <returns><b>true</b> if the conversion was successful; otherwise, <b>false</b>.</returns>
        public static bool TryParse(string version, out HarpVersion value)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            var match = VersionRegex.Match(version);
            if (match.Success && match.Groups.Count == 3)
            {
                var major = match.Groups[1].Value;
                var minor = match.Groups[2].Value;
                value = new HarpVersion(
                    major == FloatingWildcard ? (int?)null : int.Parse(major),
                    minor == FloatingWildcard ? (int?)null : int.Parse(minor));
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Converts the <see cref="HarpVersion"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>
        /// The string representation of the <see cref="HarpVersion"/> object.
        /// </returns>
        public override string ToString()
        {
            var major = Major.HasValue ? Major.Value.ToString(CultureInfo.InvariantCulture) : FloatingWildcard;
            var minor = Minor.HasValue ? Minor.Value.ToString(CultureInfo.InvariantCulture) : FloatingWildcard;
            return $"{major}.{minor}";
        }
    }
}

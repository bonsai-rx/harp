using System;
using System.Collections.Generic;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an element from an observable sequence associated with its timestamp information.
    /// </summary>
    /// <typeparam name="T">The type of the value being annotated with timestamp information.</typeparam>
    public struct Timestamped<T> : IEquatable<Timestamped<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Timestamped{T}"/> class with the specified
        /// value and timestamp information.
        /// </summary>
        /// <param name="value">The value to be annotated with timestamp information.</param>
        /// <param name="seconds">The acquisition timestamp of the value, in fractional seconds.</param>
        public Timestamped(T value, double seconds)
        {
            Value = value;
            Seconds = seconds;
        }

        /// <summary>
        /// Gets the acquisition timestamp of the value, in fractional seconds.
        /// </summary>
        public double Seconds { get; }

        /// <summary>
        /// Gets the value of the element.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Returns a value indicating whether this instance has the same value and timestamp
        /// as a specified <see cref="Timestamped{T}"/> structure.
        /// </summary>
        /// <param name="other">The <see cref="Timestamped{T}"/> structure to compare to this instance.</param>
        /// <returns>
        /// <b>true</b> if <paramref name="other"/> has the same value and timestamp as this
        /// instance; otherwise, <b>false</b>.
        /// </returns>
        public bool Equals(Timestamped<T> other)
        {
            return Seconds == other.Seconds && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// Tests to see whether the specified object is an <see cref="Timestamped{T}"/> structure
        /// with the same value and timestamp as this <see cref="Timestamped{T}"/> structure.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to test.</param>
        /// <returns>
        /// <b>true</b> if <paramref name="obj"/> is an <see cref="Timestamped{T}"/> and has the
        /// same value and timestamp as this <see cref="Timestamped{T}"/>; otherwise, <b>false</b>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Timestamped<T> timestamped && Equals(timestamped);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="Timestamped{T}"/> structure.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this <see cref="Timestamped{T}"/> structure.</returns>
        public override int GetHashCode()
        {
            return Seconds.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(Value);
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="Timestamped{T}"/>
        /// structure.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> containing the <see cref="Value"/> and <see cref="Seconds"/>
        /// properties of this <see cref="Timestamped{T}"/> structure.
        /// </returns>
        public override string ToString()
        {
            return $"{Value}@{Seconds}";
        }

        /// <summary>
        /// Tests whether two <see cref="Timestamped{T}"/> structures are equal.
        /// </summary>
        /// <param name="left">The <see cref="Timestamped{T}"/> structure on the left of the equality operator.</param>
        /// <param name="right">The <see cref="Timestamped{T}"/> structure on the right of the equality operator.</param>
        /// <returns>
        /// <b>true</b> if <paramref name="left"/> and <paramref name="right"/> have equal value and timestamp;
        /// otherwise, <b>false</b>.
        /// </returns>
        public static bool operator ==(Timestamped<T> left, Timestamped<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests whether two <see cref="Timestamped{T}"/> structures are different.
        /// </summary>
        /// <param name="left">The <see cref="Timestamped{T}"/> structure on the left of the inequality operator.</param>
        /// <param name="right">The <see cref="Timestamped{T}"/> structure on the right of the inequality operator.</param>
        /// <returns>
        /// <b>true</b> if <paramref name="left"/> and <paramref name="right"/> differ either in value or timestamp;
        /// <b>false</b> if <paramref name="left"/> and <paramref name="right"/> are equal.
        /// </returns>
        public static bool operator !=(Timestamped<T> left, Timestamped<T> right)
        {
            return !left.Equals(right);
        }
    }
}

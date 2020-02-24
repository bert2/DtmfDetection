namespace DtmfDetection {
    using System;

    /// <summary>Represents a DTMF tone in audio data.</summary>
    public readonly struct DtmfTone: IEquatable<DtmfTone> {
        /// <summary>The key of the DTMF tone.</summary>
        public readonly PhoneKey Key;

        /// <summary>The position inside the audio data where the DTMF tone was detected.</summary>
        public readonly TimeSpan Position;

        /// <summary>The length of the DTMF tone inside the audio data.</summary>
        public readonly TimeSpan Duration;

        /// <summary>The audio channel where the DTMF tone was detected.</summary>
        public readonly int Channel;

        /// <summary>Creates a new `DtmfTone` with the given identification and location.</summary>
        /// <param name="key">The key of the DTMF tone.</param>
        /// <param name="position">The position of the DTMF tone inside the audio data.</param>
        /// <param name="duration">The length of the DTMF tone.</param>
        /// <param name="channel">The audio channel of the DTMF tone.</param>
        public DtmfTone(PhoneKey key, TimeSpan position, TimeSpan duration, int channel)
            => (Key, Position, Duration, Channel) = (key, position, duration, channel);

        /// <summary>Creates a new `DtmfTone` from two `DtmfChange`s representing the start and end of the same tone.</summary>
        /// <param name="start">The `DtmfChange` that marks the start of the DTMF tone.</param>
        /// <param name="stop">The `DtmfChange` that marks the end of the DTMF tone.</param>
        /// <returns>A new `DtmfTone` that represents both `DtmfChange`s as one data structure.</returns>
        public static DtmfTone From(in DtmfChange start, in DtmfChange stop) => new DtmfTone(
            start.Key,
            start.Position,
            stop.Position - start.Position,
            start.Channel);

        /// <summary>Prints the identification and location of this `DtmfTone` to a `string` and returns it.</summary>
        /// <returns>A `string` identifiying and localizing this `DtmfTone`.</returns>
        public override string ToString() => $"{Key.ToSymbol()} @ {Position} (len: {Duration}, ch: {Channel})";

        #region Equality implementations

        /// <summary>Indicates whether the current `DtmfTone` is equal to another `DtmfTone`.</summary>
        /// <param name="other">A `DtmfTone` to compare with this `DtmfTone`.</param>
        /// <returns>Returns `true` if the current `DtmfTone` is equal to `other`; otherwise, `false`.</returns>
        public bool Equals(DtmfTone other)
            => (Key, Position, Duration, Channel) == (other.Key, other.Position, other.Duration, other.Channel);

        /// <summary>Indicates whether this `DtmfTone` and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current `DtmfTone`.</param>
        /// <returns>Returns `true` if `obj` this `DtmfTone` are the same type and represent the same value; otherwise, `false`.</returns>
        public override bool Equals(object? obj) => obj is DtmfTone other && Equals(other);

        /// <summary>Indicates whether the left-hand side `DtmfTone` is equal to the right-hand side `DtmfTone`.</summary>
        /// <param name="left">The left-hand side `DtmfTone` of the comparison.</param>
        /// <param name="right">The right-hand side `DtmfTone` of the comparison.</param>
        /// <returns>Returns `true` if the left-hand side `DtmfTone` is equal to the right-hand side `DtmfTone`; otherwise, `false`.</returns>
        public static bool operator ==(DtmfTone left, DtmfTone right) => left.Equals(right);

        /// <summary>Indicates whether the left-hand side `DtmfTone` is not equal to the right-hand side `DtmfTone`.</summary>
        /// <param name="left">The left-hand side `DtmfTone` of the comparison.</param>
        /// <param name="right">The right-hand side `DtmfTone` of the comparison.</param>
        /// <returns>Returns `true` if the left-hand side `DtmfTone` is not equal to the right-hand side `DtmfTone`; otherwise, `false`.</returns>
        public static bool operator !=(DtmfTone left, DtmfTone right) => !(left == right);

        /// <summary>Returns the hash code for this `DtmfTone`.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this `DtmfTone`.</returns>
        public override int GetHashCode() => HashCode.Combine(Key, Position, Duration, Channel);

        #endregion Equality implementations
    }
}

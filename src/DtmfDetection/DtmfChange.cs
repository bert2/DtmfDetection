namespace DtmfDetection {
    using System;

    /// <summary>Represents the start or a stop of a DTMF tone in audio data.</summary>
    public readonly struct DtmfChange : IEquatable<DtmfChange> {
        /// <summary>The key of the DTMF tone that changed.</summary>
        public readonly PhoneKey Key;

        /// <summary>The position inside the audio data where the change was detected.</summary>
        public readonly TimeSpan Position;

        /// <summary>The audio channel where the change was detected.</summary>
        public readonly int Channel;

        /// <summary>Indicates whether a DMTF tone started or stopped at the current position.</summary>
        public readonly bool IsStart;

        /// <summary>Indicates whether a DMTF tone started or stopped at the current position.</summary>
        public readonly bool IsStop => !IsStart;

        /// <summary>Creates a new `DtmfChange` with the given identification and location.</summary>
        /// <param name="key">The key of the DTMF tone.</param>
        /// <param name="position">The position of the DTMF tone inside the audio data.</param>
        /// <param name="channel">The audio channel of the DTMF tone.</param>
        /// <param name="isStart">Indicates whether a DMTF tone started or stopped at the current position.</param>
        public DtmfChange(PhoneKey key, TimeSpan position, int channel, bool isStart)
            => (Key, Position, Channel, IsStart) = (key, position, channel, isStart);

        /// <summary>Creates a new `DtmfChange` that marks the start of a DTMF tone at the specified location.</summary>
        /// <param name="key">The key of the DTMF tone.</param>
        /// <param name="position">The position of the DTMF tone inside the audio data.</param>
        /// <param name="channel">The audio channel of the DTMF tone.</param>
        /// <returns>A new `DtmfChange` marking the start of a DTMF tone.</returns>
        public static DtmfChange Start(PhoneKey key, TimeSpan position, int channel)
            => new DtmfChange(key, position, channel, isStart: true);


        /// <summary>Creates a new `DtmfChange` that marks the end of a DTMF tone at the specified location.</summary>
        /// <param name="key">The key of the DTMF tone.</param>
        /// <param name="position">The position of the DTMF tone inside the audio data.</param>
        /// <param name="channel">The audio channel of the DTMF tone.</param>
        /// <returns>A new `DtmfChange` marking the end of a DTMF tone.</returns>
        public static DtmfChange Stop(PhoneKey key, TimeSpan position, int channel)
            => new DtmfChange(key, position, channel, isStart: false);

        /// <summary>Prints the identification and location of this `DtmfChange` to a `string` and returns it.</summary>
        /// <returns>A `string` identifiying and localizing this `DtmfChange`.</returns>
        public override string ToString() => $"{Key.ToSymbol()} {(IsStart ? "started" : "stopped")} @ {Position} (ch: {Channel})";

        #region Equality implementations

        /// <summary>Indicates whether the current `DtmfChange` is equal to another `DtmfChange`.</summary>
        /// <param name="other">A `DtmfChange` to compare with this `DtmfChange`.</param>
        /// <returns>Returns `true` if the current `DtmfChange` is equal to `other`; otherwise, `false`.</returns>
        public bool Equals(DtmfChange other)
            => (Key, Position, Channel, IsStart) == (other.Key, other.Position, other.Channel, other.IsStart);

        /// <summary>Indicates whether this `DtmfChange` and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current `DtmfChange`.</param>
        /// <returns>Returns `true` if `obj` this `DtmfChange` are the same type and represent the same value; otherwise, `false`.</returns>
        public override bool Equals(object? obj) => obj is DtmfChange other && Equals(other);

        /// <summary>Indicates whether the left-hand side `DtmfChange` is equal to the right-hand side `DtmfChange`.</summary>
        /// <param name="left">The left-hand side `DtmfChange` of the comparison.</param>
        /// <param name="right">The right-hand side `DtmfChange` of the comparison.</param>
        /// <returns>Returns `true` if the left-hand side `DtmfChange` is equal to the right-hand side `DtmfChange`; otherwise, `false`.</returns>
        public static bool operator ==(DtmfChange left, DtmfChange right) => left.Equals(right);

        /// <summary>Indicates whether the left-hand side `DtmfChange` is not equal to the right-hand side `DtmfChange`.</summary>
        /// <param name="left">The left-hand side `DtmfChange` of the comparison.</param>
        /// <param name="right">The right-hand side `DtmfChange` of the comparison.</param>
        /// <returns>Returns `true` if the left-hand side `DtmfChange` is not equal to the right-hand side `DtmfChange`; otherwise, `false`.</returns>
        public static bool operator !=(DtmfChange left, DtmfChange right) => !(left == right);

        /// <summary>Returns the hash code for this `DtmfChange`.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this `DtmfChange`.</returns>
        public override int GetHashCode() => HashCode.Combine(Key, Position, Channel, IsStart);

        #endregion Equality implementations
    }
}

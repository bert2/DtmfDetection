namespace DtmfDetection {
    using System;

    public readonly struct DtmfTone: IEquatable<DtmfTone> {
        public readonly PhoneKey Key;
        public readonly TimeSpan Position;
        public readonly TimeSpan Duration;
        public readonly int Channel;

        public DtmfTone(PhoneKey key, TimeSpan position, TimeSpan duration, int channel)
            => (Key, Position, Duration, Channel) = (key, position, duration, channel);

        public static DtmfTone From(in DtmfChange start, in DtmfChange stop) => new DtmfTone(
            start.Key,
            start.Position,
            stop.Position - start.Position,
            start.Channel);

        public override string ToString() => $"{Key.ToSymbol()} @ {Position} (len: {Duration}, ch: {Channel})";

        #region Equality implementations

        public bool Equals(DtmfTone other)
            => Key == other.Key
            && Position.Equals(other.Position)
            && Duration.Equals(other.Duration)
            && Channel == other.Channel;

        public override bool Equals(object? obj) => obj is DtmfTone other && Equals(other);

        public static bool operator ==(DtmfTone left, DtmfTone right) => left.Equals(right);

        public static bool operator !=(DtmfTone left, DtmfTone right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(Key, Position, Duration, Channel);

        #endregion Equality implementations
    }
}

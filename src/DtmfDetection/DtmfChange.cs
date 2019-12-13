namespace DtmfDetection {
    using System;

    public readonly struct DtmfChange : IEquatable<DtmfChange> {
        public readonly PhoneKey Key;
        public readonly TimeSpan Position;
        public readonly int Channel;
        public readonly bool IsStart;

        public readonly bool IsStop => !IsStart;

        public DtmfChange(PhoneKey key, TimeSpan position, int channel, bool isStart)
            => (Key, Position, Channel, IsStart) = (key, position, channel, isStart);

        public static DtmfChange Start(PhoneKey key, TimeSpan position, int channel)
            => new DtmfChange(key, position, channel, isStart: true);

        public static DtmfChange Stop(PhoneKey key, TimeSpan position, int channel)
            => new DtmfChange(key, position, channel, isStart: false);

        public override string ToString() => $"{Key} {(IsStart ? "started" : "stopped")} @ {Position} (ch: {Channel})";

        #region Equality implementations

        public bool Equals(DtmfChange other)
            => Key == other.Key
            && Position.Equals(other.Position)
            && Channel == other.Channel
            && IsStart == other.IsStart;

        public override bool Equals(object? obj) => obj is DtmfChange dc && Equals(dc);

        public static bool operator ==(DtmfChange left, DtmfChange right) => left.Equals(right);

        public static bool operator !=(DtmfChange left, DtmfChange right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(Key, Position, Channel, IsStart);

        #endregion Equality implementations
    }
}

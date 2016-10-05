namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfPosition : IEquatable<DtmfPosition>
    {
        public DtmfPosition(DtmfOccurence dtmfTone, TimeSpan position, TimeSpan duration)
        {
            DtmfTone = dtmfTone.Tone;
            Channel = dtmfTone.Channel;
            Position = position;
            Duration = duration;
        }

        public DtmfTone DtmfTone { get; }

        public int Channel { get; }

        public TimeSpan Position { get; }

        public TimeSpan Duration { get; }

        public override string ToString() => $"{DtmfTone} ({Channel}) @ {Position} for {(int)Duration.TotalMilliseconds} ms";

        #region Comparison implementations

        public override bool Equals(object obj) => !ReferenceEquals(obj, null) && Equals(obj as DtmfPosition);

        public bool Equals(DtmfPosition other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return DtmfTone == other.DtmfTone
                && Position == other.Position
                && Duration == other.Duration;
        }

        public override int GetHashCode() => new { DtmfTone, Position, Duration }.GetHashCode();

        public static bool operator ==(DtmfPosition a, DtmfPosition b) => ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);

        public static bool operator !=(DtmfPosition a, DtmfPosition b) => !(a == b);

        #endregion
    }
}

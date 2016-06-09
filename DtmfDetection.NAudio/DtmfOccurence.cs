namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfOccurence : IEquatable<DtmfOccurence>
    {
        public DtmfOccurence(DtmfTone dtmfTone, TimeSpan position, TimeSpan duration)
        {
            DtmfTone = dtmfTone;
            Position = position;
            Duration = duration;
        }

        public DtmfTone DtmfTone { get; }

        public TimeSpan Position { get; }

        public TimeSpan Duration { get; }

        public override string ToString()
        {
            return DtmfTone.ToString();
        }

        #region Comparison implementations

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return Equals(obj as DtmfOccurence);
        }

        public bool Equals(DtmfOccurence other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return DtmfTone == other.DtmfTone
                && Position == other.Position
                && Duration == other.Duration;
        }

        public override int GetHashCode()
        {
            return new { DtmfTone, Position, Duration }.GetHashCode();
        }

        public static bool operator ==(DtmfOccurence a, DtmfOccurence b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);

            return a.Equals(b);
        }

        public static bool operator !=(DtmfOccurence a, DtmfOccurence b)
        {
            return !(a == b);
        }

        #endregion
    }
}

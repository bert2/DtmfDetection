namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfToneEnd : IEquatable<DtmfToneEnd>
    {
        public DtmfToneEnd(DtmfTone dtmfTone, int channel, TimeSpan duration)
        {
            DtmfTone = dtmfTone;
            Channel = channel;
            Duration = duration;
        }

        public DtmfTone DtmfTone { get; }

        public int Channel { get; }

        public TimeSpan Duration { get; }

        #region Equality implementations

        public override bool Equals(object obj) => !ReferenceEquals(obj, null) && Equals(obj as DtmfToneEnd);

        public bool Equals(DtmfToneEnd other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return DtmfTone == other.DtmfTone
                && Channel == other.Channel
                && Duration == other.Duration;
        }

        public override int GetHashCode() => new { DtmfTone, Channel, Duration }.GetHashCode();

        public static bool operator ==(DtmfToneEnd a, DtmfToneEnd b) => ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);

        public static bool operator !=(DtmfToneEnd a, DtmfToneEnd b) => !(a == b);

        #endregion
    }
}
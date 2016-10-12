namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfToneStart : IEquatable<DtmfToneStart>
    {
        public DtmfToneStart(DtmfTone dtmfTone, int channel, DateTime position)
        {
            DtmfTone = dtmfTone;
            Channel = channel;
            Position = position;
        }

        public DtmfTone DtmfTone { get; }

        public int Channel { get; }

        public DateTime Position { get; }

        #region Equality implementations

        public override bool Equals(object obj) => !ReferenceEquals(obj, null) && Equals(obj as DtmfToneStart);

        public bool Equals(DtmfToneStart other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return DtmfTone == other.DtmfTone
                && Channel == other.Channel
                && Position == other.Position;
        }

        public override int GetHashCode() => new { DtmfTone, Channel, Position }.GetHashCode();

        public static bool operator ==(DtmfToneStart a, DtmfToneStart b) => ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);

        public static bool operator !=(DtmfToneStart a, DtmfToneStart b) => !(a == b);

        #endregion
    }
}
namespace DtmfDetection
{
    using System;
    public class DtmfOccurence : IEquatable<DtmfOccurence>
    {
        public DtmfOccurence(DtmfTone tone, int channel)
        {
            Tone = tone;
            Channel = channel;
        }

        public DtmfTone Tone { get; }

        public int Channel { get; }

        public override string ToString() => $"{Tone} ({Channel})";

        #region Comparison implementations

        public override bool Equals(object obj) => !ReferenceEquals(obj, null) && Equals(obj as DtmfOccurence);

        public bool Equals(DtmfOccurence other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return Channel == other.Channel
                && Tone == other.Tone;
        }

        public override int GetHashCode() => new { Tone, Channel }.GetHashCode();

        public static bool operator ==(DtmfOccurence a, DtmfOccurence b) => ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);

        public static bool operator !=(DtmfOccurence a, DtmfOccurence b) => !(a == b);

        #endregion
    }
}
namespace DtmfDetection.NAudio
{
    using System;

    /// <summary>Represents the start of a DTMF tone in captured live audio.</summary>
    public class DtmfToneStart : IEquatable<DtmfToneStart>
    {
        /// <summary>Creates a new instance of DtmfToneStart.</summary>
        /// <param name="dtmfTone">The DTMF tone.</param>
        /// <param name="channel">The audio channel where the DTMF tone occured.</param>
        /// <param name="position">The estimated time when the DTMF tone occured.</param>
        public DtmfToneStart(DtmfTone dtmfTone, int channel, DateTime position)
        {
            DtmfTone = dtmfTone;
            Channel = channel;
            Position = position;
        }

        /// <summary>The detected DTMF tone.</summary>
        public DtmfTone DtmfTone { get; }

        /// <summary>The audio channel where the DTMF tone started. 0 for left, 1 for right etc.</summary>
        public int Channel { get; }

        /// <summary>The estimated time when the DTMF tone started.</summary>
        /// <remarks>The accuracy of the estimation depends on the sample rate f_s of the audio data and is 
        /// calculated by 205 / f_s Hz * 1000 ms  (e.g. for a  sample rate f_s of 8000 Hz it is 25.625 ms).</remarks>
        public DateTime Position { get; }

        /// <inheritdoc />
        public override string ToString() =>  $"{DtmfTone} started @ {Position} ({Channel})";

        #region Equality implementations

        /// <inheritdoc />
        public override bool Equals(object obj) => !ReferenceEquals(obj, null) && Equals(obj as DtmfToneStart);

        /// <inheritdoc />
        public bool Equals(DtmfToneStart other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return DtmfTone == other.DtmfTone
                && Channel == other.Channel
                && Position == other.Position;
        }

        /// <inheritdoc />
        public override int GetHashCode() => new { DtmfTone, Channel, Position }.GetHashCode();

        /// <summary>Compares two DtmfToneStart's for equality.</summary>
        /// <param name="a">The left-hand side DtmfToneStart.</param>
        /// <param name="b">The right-hand side DtmfToneStart.</param>
        /// <returns>True if the DtmfToneStart's a and b are equal, false otherwise.</returns>
        public static bool operator ==(DtmfToneStart a, DtmfToneStart b) => ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);

        /// <summary>Compares two DtmfToneStart's for inequality.</summary>
        /// <param name="a">The left-hand side DtmfToneStart.</param>
        /// <param name="b">The right-hand side DtmfToneStart.</param>
        /// <returns>True if the DtmfToneStart's a and b are not equal, false otherwise.</returns>
        public static bool operator !=(DtmfToneStart a, DtmfToneStart b) => !(a == b);

        #endregion
    }
}
namespace DtmfDetection.NAudio
{
    using System;

    /// <summary>Represents the end of a DTMF tone in captured live audio.</summary>
    public class DtmfToneEnd : IEquatable<DtmfToneEnd>
    {
        /// <summary>Creates a new instance of DtmfToneEnd.</summary>
        /// <param name="dtmfTone">The DTMF tone.</param>
        /// <param name="channel">The audio channel where the DTMF tone occured.</param>
        /// <param name="duration">The estimated duration of the DTMF tone.</param>
        public DtmfToneEnd(DtmfTone dtmfTone, int channel, TimeSpan duration)
        {
            DtmfTone = dtmfTone;
            Channel = channel;
            Duration = duration;
        }

        /// <summary>The detected DTMF tone.</summary>
        public DtmfTone DtmfTone { get; }

        /// <summary>The audio channel where the DTMF tone occured. 0 for left, 1 for right etc.</summary>
        public int Channel { get; }

        /// <summary>The estimated duration of the DTMF tone.</summary>
        /// <remarks>The accuracy of the estimation depends on the sample rate f_s of the audio data and is 
        /// calculated by 205 / f_s Hz * 1000 ms  (e.g. for a  sample rate f_s of 8000 Hz it is 25.625 ms).</remarks>
        public TimeSpan Duration { get; }

        /// <inheritdoc />
        public override string ToString() => $"{DtmfTone} stopped after {Duration.TotalMilliseconds} ms ({Channel})";

        #region Equality implementations

        /// <inheritdoc />
        public override bool Equals(object obj) => !ReferenceEquals(obj, null) && Equals(obj as DtmfToneEnd);

        /// <inheritdoc />
        public bool Equals(DtmfToneEnd other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return DtmfTone == other.DtmfTone
                && Channel == other.Channel
                && Duration == other.Duration;
        }

        /// <inheritdoc />
        public override int GetHashCode() => new { DtmfTone, Channel, Duration }.GetHashCode();

        /// <summary>Compares two DtmfToneEnd's for equality.</summary>
        /// <param name="a">The left-hand side DtmfToneEnd.</param>
        /// <param name="b">The right-hand side DtmfToneEnd.</param>
        /// <returns>True if the DtmfToneEnd's a and b are equal, false otherwise.</returns>
        public static bool operator ==(DtmfToneEnd a, DtmfToneEnd b) => ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);

        /// <summary>Compares two DtmfToneEnd's for inequality.</summary>
        /// <param name="a">The left-hand side DtmfToneEnd.</param>
        /// <param name="b">The right-hand side DtmfToneEnd.</param>
        /// <returns>True if the DtmfToneEnd's a and b are not equal, false otherwise.</returns>
        public static bool operator !=(DtmfToneEnd a, DtmfToneEnd b) => !(a == b);

        #endregion
    }
}
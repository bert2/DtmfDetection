namespace DtmfDetection.NAudio
{
    using System;

    /// <summary>Represents a DTMF tone in an audio file or stream.</summary>
    public class DtmfOccurence : IEquatable<DtmfOccurence>
    {
        /// <summary>Creates a new instance of DtmfOccurence.</summary>
        /// <param name="dtmfTone">The DTMF tone.</param>
        /// <param name="channel">The audio channel where the DTMF tone occured.</param>
        /// <param name="position">The estimated position in the file or stream where the DTMF tone occured.</param>
        /// <param name="duration">The estimated duration of the DTMF tone.</param>
        public DtmfOccurence(DtmfTone dtmfTone, int channel, TimeSpan position, TimeSpan duration)
        {
            DtmfTone = dtmfTone;
            Channel = channel;
            Position = position;
            Duration = duration;
        }

        /// <summary>The detected DTMF tone.</summary>
        public DtmfTone DtmfTone { get; }

        /// <summary>The audio channel where the DTMF tone occured. 0 for left, 1 for right etc.</summary>
        public int Channel { get; }

        /// <summary>The estimated position in the file or stream where the DTMF tone occured.</summary>
        /// <remarks>The accuracy of the estimation depends on the sample rate f_s of the audio data and is 
        /// calculated by 205 / f_s Hz * 1000 ms  (e.g. for a  sample rate f_s of 8000 Hz it is 25.625 ms).</remarks>
        public TimeSpan Position { get; }

        /// <summary>The estimated duration of the DTMF tone.</summary>
        public TimeSpan Duration { get; }

        /// <inheritdoc />
        public override string ToString() => $"{DtmfTone} ({Channel}) @ {Position} for {(int)Duration.TotalMilliseconds} ms";

        #region Equality implementations

        /// <inheritdoc />
        public override bool Equals(object obj) => !ReferenceEquals(obj, null) && Equals(obj as DtmfOccurence);

        /// <inheritdoc />
        public bool Equals(DtmfOccurence other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return DtmfTone == other.DtmfTone
                && Channel == other.Channel
                && Position == other.Position
                && Duration == other.Duration;
        }

        /// <inheritdoc />
        public override int GetHashCode() => new { DtmfTone, Channel, Position, Duration }.GetHashCode();

        /// <summary>Compares two DtmfOccurence's for equality.</summary>
        /// <param name="a">The left-hand side DtmfOccurence.</param>
        /// <param name="b">The right-hand side DtmfOccurence.</param>
        /// <returns>True if the DtmfOccurence's a and b are equal, false otherwise.</returns>
        public static bool operator ==(DtmfOccurence a, DtmfOccurence b) => ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);

        /// <summary>Compares two DtmfOccurence's for inequality.</summary>
        /// <param name="a">The left-hand side DtmfOccurence.</param>
        /// <param name="b">The right-hand side DtmfOccurence.</param>
        /// <returns>True if the DtmfOccurence's a and b are not equal, false otherwise.</returns>
        public static bool operator !=(DtmfOccurence a, DtmfOccurence b) => !(a == b);

        #endregion
    }
}

namespace DtmfDetection {
    using System;

    /// <summary>The detector configuration.</summary>
    public readonly struct Config : IEquatable<Config> {
        /// <summary>The default detection threshold (tuned to normalized responses).</summary>
        public const double DefaultThreshold = 30;

        /// <summary>The default number of samples to analyze before the Goertzel response should be calulated (tuned to minimize error of the target frequency bin).</summary>
        public const int DefaultSampleBlockSize = 205;

        /// <summary>Default rate (in Hz) at which the analyzed samples are expected to have been measured.</summary>
        public const int DefaultSampleRate = 8000;

        /// <summary>A default configuration instance.</summary>
        public static readonly Config Default = new Config(DefaultThreshold, DefaultSampleBlockSize, DefaultSampleRate, normalizeResponse: true);

        /// <summary>The detection threshold. Typical values are `30`-`35` (when `NormalizeResponse` is `true`) and `100`-`115` (when `NormalizeResponse` is `false`).</summary>
        public readonly double Threshold;

        /// <summary>The number of samples to analyze before the Goertzel response should be calulated. It is recommened to leave it at the default value `205` (tuned to minimize error of the target frequency bin).</summary>
        public readonly int SampleBlockSize;

        /// <summary>The sample rate (in Hz) the Goertzel algorithm expects. Sources with higher samples rates must resampled to this sample rate. It is recommended to leave it at the default value `8000`.</summary>
        public readonly int SampleRate;

        /// <summary>Toggles normalization of the Goertzel response with the total signal energy of the sample block. Recommended setting is `true` as this provides invariance to loudness changes of the signal.</summary>
        public readonly bool NormalizeResponse;

        /// <summary>Creates a new `Config` instance.</summary>
        /// <param name="threshold">The detection threshold. Typical values are `30`-`35` (when `normalizeResponse` is `true`) and `100`-`115` (when `normalizeResponse` is `false`).</param>
        /// <param name="sampleBlockSize">The number of samples to analyze before the Goertzel response should be calulated. It is recommened to leave it at the default value `205` (tuned to minimize error of the target frequency bin).</param>
        /// <param name="sampleRate">The sample rate (in Hz) the Goertzel algorithm expects. Sources with higher samples rates must resampled to this sample rate. It is recommended to leave it at the default value `8000`.</param>
        /// <param name="normalizeResponse">Toggles normalization of the Goertzel response with the total signal energy of the sample block. Recommended setting is `true` as this provides invariance to loudness changes of the signal.</param>
        public Config(double threshold, int sampleBlockSize, int sampleRate, bool normalizeResponse)
            => (Threshold, SampleBlockSize, SampleRate, NormalizeResponse) = (threshold, sampleBlockSize, sampleRate, normalizeResponse);

        /// <summary>Creates a cloned `Config` instance from this instance, but with a new `Threshold` setting.</summary>
        /// <param name="threshold">The detection threshold. Typical values are `30`-`35` (when `normalizeResponse` is `true`) and `100`-`115` (when `normalizeResponse` is `false`).</param>
        /// <returns>A new `Config` instance with the specified `Threshold` setting.</returns>
        public Config WithThreshold(double threshold) => new Config(threshold, SampleBlockSize, SampleRate, NormalizeResponse);

        /// <summary>Creates a cloned `Config` instance from this instance, but with a new `SampleBlockSize` setting.</summary>
        /// <param name="sampleBlockSize">The number of samples to analyze before the Goertzel response should be calulated. It is recommened to leave it at the default value `205` (tuned to minimize error of the target frequency bin).</param>
        /// <returns>A new `Config` instance with the specified `SampleBlockSize` setting.</returns>
        public Config WithSampleBlockSize(int sampleBlockSize) => new Config(Threshold, sampleBlockSize, SampleRate, NormalizeResponse);

        /// <summary>Creates a cloned `Config` instance from this instance, but with a new `SampleRate` setting.</summary>
        /// <param name="sampleRate">The sample rate (in Hz) the Goertzel algorithm expects. Sources with higher samples rates must resampled to this sample rate. It is recommended to leave it at the default value `8000`.</param>
        /// <returns>A new `Config` instance with the specified `SampleRate` setting.</returns>
        public Config WithSampleRate(int sampleRate) => new Config(Threshold, SampleBlockSize, sampleRate, NormalizeResponse);

        /// <summary>Creates a cloned `Config` instance from this instance, but with a new `NormalizeResponse` setting.</summary>
        /// <param name="normalizeResponse">Toggles normalization of the Goertzel response with the total signal energy of the sample block. Recommended setting is `true` as this provides invariance to loudness changes of the signal.</param>
        /// <returns>A new `Config` instance with the specified `NormalizeResponse` setting.</returns>
        public Config WithNormalizeResponse(bool normalizeResponse) => new Config(Threshold, SampleBlockSize, SampleRate, normalizeResponse);

        #region Equality implementations

        /// <summary>Indicates whether the current `Config` is equal to another `Config`.</summary>
        /// <param name="other">A `Config` to compare with this `Config`.</param>
        /// <returns>Returns `true` if the current `Config` is equal to `other`; otherwise, `false`.</returns>
        public bool Equals(Config other) =>
            (Threshold, SampleBlockSize, SampleRate, NormalizeResponse)
            == (other.Threshold, other.SampleBlockSize, other.SampleRate, other.NormalizeResponse);

        /// <summary>Indicates whether this `Config` and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current `Config`.</param>
        /// <returns>Returns `true` if `obj` this `Config` are the same type and represent the same value; otherwise, `false`.</returns>
        public override bool Equals(object? obj) => obj is Config other && Equals(other);

        /// <summary>Indicates whether the left-hand side `Config` is equal to the right-hand side `Config`.</summary>
        /// <param name="left">The left-hand side `Config` of the comparison.</param>
        /// <param name="right">The right-hand side `Config` of the comparison.</param>
        /// <returns>Returns `true` if the left-hand side `Config` is equal to the right-hand side `Config`; otherwise, `false`.</returns>
        public static bool operator ==(Config left, Config right) => left.Equals(right);

        /// <summary>Indicates whether the left-hand side `Config` is not equal to the right-hand side `Config`.</summary>
        /// <param name="left">The left-hand side `Config` of the comparison.</param>
        /// <param name="right">The right-hand side `Config` of the comparison.</param>
        /// <returns>Returns `true` if the left-hand side `Config` is not equal to the right-hand side `Config`; otherwise, `false`.</returns>
        public static bool operator !=(Config left, Config right) => !(left == right);

        /// <summary>Returns the hash code for this `Config`.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this `Config`.</returns>
        public override int GetHashCode() => HashCode.Combine(Threshold, SampleBlockSize, SampleRate, NormalizeResponse);

        #endregion Equality implementations
    }
}

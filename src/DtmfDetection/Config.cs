namespace DtmfDetection {
    using System;

    public readonly struct Config : IEquatable<Config> {
        public const double DefaultThreshold = 30;
        public const int DefaultSampleBlockSize = 205;
        public const int DefaultSampleRate = 8000;

        public static readonly Config Default = new Config(DefaultThreshold, DefaultSampleBlockSize, DefaultSampleRate, normalizeResponse: true);

        public readonly double Threshold;
        public readonly int SampleBlockSize;
        public readonly int SampleRate;
        public readonly bool NormalizeResponse;

        public Config(double threshold, int sampleBlockSize, int sampleRate, bool normalizeResponse)
            => (Threshold, SampleBlockSize, SampleRate, NormalizeResponse) = (threshold, sampleBlockSize, sampleRate, normalizeResponse);

        public Config WithThreshold(double threshold) => new Config(threshold, SampleBlockSize, SampleRate, NormalizeResponse);

        public Config WithSampleBlockSize(int sampleBlockSize) => new Config(Threshold, sampleBlockSize, SampleRate, NormalizeResponse);

        public Config WithSampleRate(int sampleRate) => new Config(Threshold, SampleBlockSize, sampleRate, NormalizeResponse);

        public Config WithNormalizeResponse(bool normalizeResponse) => new Config(Threshold, SampleBlockSize, SampleRate, normalizeResponse);

        #region Equality implementations

        public bool Equals(Config other)
            => Threshold == other.Threshold
            && SampleBlockSize == other.SampleBlockSize
            && SampleRate == other.SampleRate
            && NormalizeResponse == other.NormalizeResponse;

        public override bool Equals(object? obj) => obj is Config other && Equals(other);

        public static bool operator ==(Config left, Config right) => left.Equals(right);

        public static bool operator !=(Config left, Config right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(Threshold, SampleBlockSize, SampleRate, NormalizeResponse);

        #endregion Equality implementations
    }
}

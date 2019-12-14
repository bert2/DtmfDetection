namespace DtmfDetection {
    using System;

    public readonly struct Config : IEquatable<Config> {
        public static readonly Config Default = new Config(35.0, 205, 8000);
        public readonly double Threshold;
        public readonly int SampleBlockSize;
        public readonly int SampleRate;

        public Config(double threshold = 35.0, int sampleBlockSize = 205, int sampleRate = 8000)
            => (Threshold, SampleBlockSize, SampleRate) = (threshold, sampleBlockSize, sampleRate);

        public Config WithThreshold(double threshold) => new Config(threshold, SampleBlockSize, SampleRate);

        public Config WithSampleBlockSize(int sampleBlockSize) => new Config(Threshold, sampleBlockSize, SampleRate);

        public Config WithSampleRate(int sampleRate) => new Config(Threshold, SampleBlockSize, sampleRate);

        #region Equality implementations

        public bool Equals(Config other)
            => Threshold == other.Threshold
            && SampleBlockSize == other.SampleBlockSize
            && SampleRate == other.SampleRate;

        public override bool Equals(object? obj) => obj is Config other && Equals(other);

        public static bool operator ==(Config left, Config right) => left.Equals(right);

        public static bool operator !=(Config left, Config right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(Threshold, SampleBlockSize, SampleRate);

        #endregion Equality implementations
    }
}

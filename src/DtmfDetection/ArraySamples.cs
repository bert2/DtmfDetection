namespace DtmfDetection {
    using System;

    public class ArraySamples : ISamples {
        private readonly float[] samples;
        private long position;

        public int Channels { get; }

        public int SampleRate { get; }

        public TimeSpan Position => new TimeSpan((long)Math.Round(position / Channels * 1000.0 / SampleRate));

        public ArraySamples(float[] samples, int numChannels, int sampleRate)
            => (this.samples, Channels, SampleRate) = (samples, numChannels, sampleRate);

        public int ReadNextBlock(float[] buffer, int count) {
            var safeCount = (int)Math.Min(count, samples.LongLength - position);
            Array.Copy(samples, position, buffer, 0, safeCount);
            position += safeCount;
            return safeCount;
        }
    }
}

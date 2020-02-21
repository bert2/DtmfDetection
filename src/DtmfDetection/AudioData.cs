namespace DtmfDetection {
    using System;
    using DtmfDetection.Interfaces;

    public class AudioData : ISamples {
        private readonly float[] samples;
        private long position;

        public int Channels { get; }

        public int SampleRate { get; }

        public TimeSpan Position => new TimeSpan((long)Math.Round(position / Channels * 1000.0 / SampleRate));

        public AudioData(float[] samples, int channels, int sampleRate)
            => (this.samples, Channels, SampleRate) = (samples, channels, sampleRate);

        public int Read(float[] buffer, int count) {
            var safeCount = (int)Math.Min(count, samples.LongLength - position);
            Array.Copy(samples, position, buffer, 0, safeCount);
            position += safeCount;
            return safeCount;
        }
    }
}

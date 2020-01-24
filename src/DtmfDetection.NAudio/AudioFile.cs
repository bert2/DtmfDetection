namespace DtmfDetection.NAudio {
    using System;
    using global::NAudio.Wave;

    public class AudioFile : ISamples {
        private readonly WaveStream source;
        private readonly ISampleProvider samples;

        public int Channels { get; }

        public int SampleRate { get; }

        public TimeSpan Position => source.CurrentTime;

        public AudioFile(WaveStream source, int maxSampleRate, bool forceMono = true) {
            this.source = source;
            Channels = forceMono ? 1 : source.WaveFormat.Channels;
            SampleRate = Math.Min(source.WaveFormat.SampleRate, maxSampleRate);

            var samples = forceMono ? source.ToSampleProvider().AsMono() : source.ToSampleProvider();
            this.samples = samples.Downsample(maxSampleRate);
        }

        public int ReadNextBlock(float[] buffer, int count) => samples.Read(buffer, 0, count);
    }
}

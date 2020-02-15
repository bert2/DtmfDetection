namespace DtmfDetection.NAudio {
    using System;
    using DtmfDetection.Interfaces;
    using global::NAudio.Wave;

    public class AudioFile : ISamples {
        private readonly WaveStream source;
        private readonly ISampleProvider samples;

        public int Channels => samples.WaveFormat.Channels;

        public int SampleRate => samples.WaveFormat.SampleRate;

        public TimeSpan Position => source.CurrentTime;

        public AudioFile(WaveStream source, int targetSampleRate, bool forceMono = true) {
            this.source = source;

            var samples = forceMono ? source.ToSampleProvider().AsMono() : source.ToSampleProvider();
            this.samples = samples.Resample(targetSampleRate);
        }

        public int Read(float[] buffer, int count) => samples.Read(buffer, 0, count);
    }
}

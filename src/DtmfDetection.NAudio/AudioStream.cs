namespace DtmfDetection.NAudio {
    using System;
    using DtmfDetection.Interfaces;
    using global::NAudio.Wave;

    public class AudioStream : ISamples {
        private readonly BufferedWaveProvider source;
        private readonly ISampleProvider samples;

        public int Channels => samples.WaveFormat.Channels;

        public int SampleRate => samples.WaveFormat.SampleRate;

        public TimeSpan Position => DateTime.Now.TimeOfDay;

        public AudioStream(IWaveIn source, int targetSampleRate, bool forceMono = true) {
            this.source = source.ToBufferedWaveProvider();
            var samples = forceMono ? this.source.ToSampleProvider().AsMono() : this.source.ToSampleProvider();
            this.samples = samples.Resample(targetSampleRate);
        }

        public int Read(float[] buffer, int count) {
            source.WaitForSamples(count);
            return samples.Read(buffer, 0, count);
        }
    }
}

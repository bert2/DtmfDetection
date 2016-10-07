namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public class StreamingSampleSource : ISampleSource
    {
        private readonly BufferedWaveProvider sourceBuffer;

        private readonly ISampleProvider samples;

        public StreamingSampleSource(DetectorConfig config, BufferedWaveProvider source, bool forceMono = true)
        {
            sourceBuffer = source;

            var sampleProvider = source.ToSampleProvider();

            if (forceMono)
                sampleProvider = sampleProvider.AsMono();

            samples = sampleProvider.DownsampleTo(config.MaxSampleRate);
        }

        public bool HasSamples { get; } = true;

        public int SampleRate => samples.WaveFormat.SampleRate;

        public int Channels => samples.WaveFormat.Channels;

        public IEnumerable<float> Samples
        {
            get
            {
                var buffer = new float[Channels];

                while (HasSamples)
                {
                    sourceBuffer.WaitForSample();
                    var count = samples.Read(buffer, 0, Channels);

                    if (count <= 0)
                        continue;

                    for (var channel = 0; channel < Channels; channel++)
                        yield return buffer[channel];
                }
            }
        }
    }
}
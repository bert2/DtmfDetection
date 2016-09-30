namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public class StreamingSampleSource : ISampleSource
    {
        private readonly BufferedWaveProvider sourceBuffer;

        private readonly ISampleProvider samples;

        public StreamingSampleSource(DetectorConfig config, BufferedWaveProvider source)
        {
            sourceBuffer = source;
            samples = source.ToSampleProvider().AsMono().DownsampleTo(config.MaxSampleRate);
        }

        public bool HasSamples { get; } = true;

        public int SampleRate => samples.WaveFormat.SampleRate;

        public IEnumerable<float> Samples
        {
            get
            {
                var buffer = new float[1];

                while (HasSamples)
                {
                    sourceBuffer.WaitForSample();
                    var count = samples.Read(buffer, 0, 1);

                    if (count > 0)
                        yield return buffer[0];
                }
            }
        }
    }
}
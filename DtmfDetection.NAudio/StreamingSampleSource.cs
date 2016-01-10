namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public class StreamingSampleSource : ISampleSource
    {
        private readonly BufferedWaveProvider sourceBuffer;

        private readonly ISampleProvider samples;

        public StreamingSampleSource(BufferedWaveProvider source)
        {
            sourceBuffer = source;
            samples = source.ToSampleProvider().AsMono().SampleWith(DtmfDetector.SampleRate);
        }

        public bool HasSamples { get; } = true;

        public IEnumerable<float> Samples
        {
            get
            {
                var buffer = new float[1];

                while (HasSamples)
                {
                    sourceBuffer.WaitForSample();
                    samples.Read(buffer, 0, 1);
                    yield return buffer[0];
                }
            }
        }
    }
}
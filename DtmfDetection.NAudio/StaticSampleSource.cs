namespace DtmfDetection.NAudio
{
    using System;
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public class StaticSampleSource : ISampleSource
    {
        private readonly SampleBlockProvider samples;

        public StaticSampleSource(WaveFileReader source)
        {
            samples = source.AsMono()
                            .SampleWith(DtmfDetector.SampleRate)
                            .Blockwise(DtmfDetector.SampleBlockSize);
        }

        public bool HasSamples { get; private set; } = true;

        public IEnumerable<float> Samples
        {
            get
            {
                if (!HasSamples)
                    throw new InvalidOperationException("No more data available");

                HasSamples = samples.ReadNextBlock();
                return samples.CurrentBlock;
            }
        }
    }
}
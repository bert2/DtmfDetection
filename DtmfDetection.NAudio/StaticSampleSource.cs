namespace DtmfDetection.NAudio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::NAudio.Wave;

    public class StaticSampleSource : ISampleSource
    {
        private readonly SampleBlockProvider samples;

        private int numSamplesRead;

        public StaticSampleSource(DetectorConfig config, IWaveProvider source, bool forceMono = true)
        {
            var sampleProvider = source.ToSampleProvider();

            if (forceMono)
                sampleProvider = sampleProvider.AsMono();

            samples = sampleProvider.DownsampleTo(config.MaxSampleRate).Blockwise(config.SampleBlockSize);

            // Optimistically assume that we are going to read at least BlockSize bytes.
            numSamplesRead = samples.BlockSize;
        }

        public bool HasSamples => numSamplesRead >= samples.BlockSize;

        public int SampleRate => samples.SampleRate;

        public int Channels => samples.Channels;

        public IEnumerable<float> Samples
        {
            get
            {
                if (!HasSamples)
                    throw new InvalidOperationException("No more data available");

                numSamplesRead = samples.ReadNextBlock();
                return samples.CurrentBlock.Take(numSamplesRead);
            }
        }
    }
}
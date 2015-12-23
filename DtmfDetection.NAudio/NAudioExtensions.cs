namespace DtmfDetection.NAudio
{
    using System.Threading;

    using global::NAudio.Wave;
    using global::NAudio.Wave.SampleProviders;

    public static class NAudioExtensions
    {
        public static ISampleProvider SampleWith(this ISampleProvider source, int sampleRate)
        {
            return source.WaveFormat.SampleRate != sampleRate
                       ? new WdlResamplingSampleProvider(source, sampleRate)
                       : source;
        }

        public static ISampleProvider AsMono(this IWaveProvider source)
        {
            return source.WaveFormat.Channels != 1
                       ? new MultiplexingSampleProvider(new[] { source.ToSampleProvider() }, 1)
                       : source.ToSampleProvider();
        }

        public static SampleBlockProvider Blockwise(this ISampleProvider source, int blockSize)
        {
            return new SampleBlockProvider(source, blockSize);
        }

        public static void WaitForSample(this BufferedWaveProvider source)
        {
            var bytesPerSample = source.WaveFormat.BitsPerSample / 8 * source.WaveFormat.Channels;

            while (source.BufferedBytes < bytesPerSample)
            {
                Thread.Sleep(1);
            }

        }
    }
}
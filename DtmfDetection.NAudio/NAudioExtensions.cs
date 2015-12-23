namespace DtmfDetection.NAudio
{
    using System;
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

        public static void WaitForData(this BufferedWaveProvider source, TimeSpan requiredDuration)
        {
            while (source.BufferedDuration < requiredDuration)
            {
                Thread.Sleep(requiredDuration - source.BufferedDuration);
            }
        }
    }
}
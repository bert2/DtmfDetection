namespace DtmfDetection.NAudio
{
    using System.Threading;

    using global::NAudio.Wave;

    public static class BufferedWaveProviderExtensions
    {
        public static void WaitForSample(this BufferedWaveProvider source)
        {
            var bytesPerSample = source.WaveFormat.BitsPerSample / 8 * source.WaveFormat.Channels;

            while (source.BufferedBytes < bytesPerSample)
                Thread.Sleep(1);
        }
    }
}
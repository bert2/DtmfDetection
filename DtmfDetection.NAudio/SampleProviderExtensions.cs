namespace DtmfDetection.NAudio
{
    using global::NAudio.Wave;
    using global::NAudio.Wave.SampleProviders;

    public static class SampleProviderExtensions
    {
        public static ISampleProvider DownsampleTo(this ISampleProvider source, int maxSampleRate)
        {
            return source.WaveFormat.SampleRate > maxSampleRate
                       ? new WdlResamplingSampleProvider(source, maxSampleRate)
                       : source;
        }

        public static ISampleProvider AsMono(this ISampleProvider source)
        {
            return source.WaveFormat.Channels > 1
                ? new MonoSampleProvider(source)
                : source;
        }

        public static SampleBlockProvider Blockwise(this ISampleProvider source, int blockSize)
            => new SampleBlockProvider(source, blockSize);
    }
}
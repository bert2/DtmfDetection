namespace DtmfDetection.NAudio
{
    using global::NAudio.Wave;
    using global::NAudio.Wave.SampleProviders;

    public static class SampleProviderExtensions
    {
        public static ISampleProvider SampleWith(this ISampleProvider source, int sampleRate)
        {
            return source.WaveFormat.SampleRate != sampleRate
                       ? new WdlResamplingSampleProvider(source, sampleRate)
                       : source;
        }

        public static ISampleProvider AsMono(this ISampleProvider source)
        {
            return source.WaveFormat.Channels != 1
                       ? new MultiplexingSampleProvider(new[] { source }, 1)
                       : source;
        }

        public static SampleBlockProvider Blockwise(this ISampleProvider source, int blockSize)
        {
            return new SampleBlockProvider(source, blockSize);
        }
    }
}
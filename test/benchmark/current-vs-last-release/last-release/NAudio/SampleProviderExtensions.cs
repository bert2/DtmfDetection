namespace DtmfDetection.NAudio.LastRelease {
    using System.Diagnostics.CodeAnalysis;
    using global::NAudio.Wave;
    using global::NAudio.Wave.SampleProviders;

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public static class SampleProviderExtensions
    {
        public static ISampleProvider DownsampleTo(this ISampleProvider source, int maxSampleRate)
            => source.WaveFormat.SampleRate > maxSampleRate
                    ? new WdlResamplingSampleProvider(source, maxSampleRate)
                    : source;

        public static ISampleProvider AsMono(this ISampleProvider source)
            => source.WaveFormat.Channels > 1
                ? new MonoSampleProvider(source)
                : source;

        public static SampleBlockProvider Blockwise(this ISampleProvider source, int blockSize)
            => new SampleBlockProvider(source, blockSize);
    }
}
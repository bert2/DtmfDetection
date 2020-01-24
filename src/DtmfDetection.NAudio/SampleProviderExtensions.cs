namespace DtmfDetection.NAudio {
    using global::NAudio.Wave;
    using global::NAudio.Wave.SampleProviders;

    public static class SampleProviderExtensions {
        public static ISampleProvider Downsample(this ISampleProvider source, int maxSampleRate)
            => source.WaveFormat.SampleRate > maxSampleRate
                ? new WdlResamplingSampleProvider(source, maxSampleRate)
                : source;

        public static ISampleProvider AsMono(this ISampleProvider source)
            => source.WaveFormat.Channels > 1
                ? new MonoSampleProvider(source)
                : source;
    }
}

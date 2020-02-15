namespace DtmfDetection.NAudio {
    using global::NAudio.Wave;
    using global::NAudio.Wave.SampleProviders;

    public static class SampleProviderExt {
        public static ISampleProvider Resample(this ISampleProvider source, int targetSampleRate)
            => source.WaveFormat.SampleRate != targetSampleRate
                ? new WdlResamplingSampleProvider(source, targetSampleRate)
                : source;

        public static ISampleProvider AsMono(this ISampleProvider source)
            => source.WaveFormat.Channels > 1
                ? new MonoSampleProvider(source)
                : source;
    }
}

namespace DtmfDetection.NAudio {
    using global::NAudio.Wave;
    using global::NAudio.Wave.SampleProviders;

    /// <summary>Provides extensions methods for `ISampleProvider`s.</summary>
    public static class SampleProviderExt {
        /// <summary>Resamples the input data to the specified target sample rate using the `WdlResamplingSampleProvider`.
        /// Does nothing in case the sample rate already matches.</summary>
        /// <param name="source">The `ISampleProvider` providing the source samples.</param>
        /// <param name="targetSampleRate">The sample rate to convert the provided samples to.</param>
        /// <returns>A new `ISampleProvider` having the specified target sample rate.</returns>
        public static ISampleProvider Resample(this ISampleProvider source, int targetSampleRate)
            => source.WaveFormat.SampleRate != targetSampleRate
                ? new WdlResamplingSampleProvider(source, targetSampleRate)
                : source;

        /// <summary>Converts multi-channel input data to mono by avering all channels. Does nothing in case the input data already is mono.</summary>
        /// <param name="source">The `ISampleProvider` providing the source samples.</param>
        /// <returns>A new `ISampleProvider` having only one channel.</returns>
        public static ISampleProvider AsMono(this ISampleProvider source)
            => source.WaveFormat.Channels > 1
                ? new MonoSampleProvider(source)
                : source;
    }
}

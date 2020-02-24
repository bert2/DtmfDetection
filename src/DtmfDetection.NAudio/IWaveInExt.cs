namespace DtmfDetection.NAudio {
    using global::NAudio.Wave;

    /// <summary>Provides an extensions method to buffer a `IWaveIn` stream using a `BufferedWaveProvider`.</summary>
    public static class IWaveInExt {
        /// <summary>Creates a `BufferedWaveProvider` for a `IWaveIn` and returns it.</summary>
        /// <param name="source">The `IWaveIn` stream providing the input data.</param>
        /// <returns>The `BufferedWaveProvider` buffering the input stream.</returns>
        public static BufferedWaveProvider ToBufferedWaveProvider(this IWaveIn source) {
            var buffer = new BufferedWaveProvider(source.WaveFormat) { DiscardOnBufferOverflow = true };
            source.DataAvailable += (sender, e) => buffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
            return buffer;
        }
    }
}

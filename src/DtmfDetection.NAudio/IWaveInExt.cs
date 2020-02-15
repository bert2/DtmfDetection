namespace DtmfDetection.NAudio {
    using global::NAudio.Wave;

    public static class IWaveInExt {
        public static BufferedWaveProvider ToBufferedWaveProvider(this IWaveIn source) {
            var buffer = new BufferedWaveProvider(source.WaveFormat) { DiscardOnBufferOverflow = true };
            source.DataAvailable += (sender, e) => buffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
            return buffer;
        }
    }
}

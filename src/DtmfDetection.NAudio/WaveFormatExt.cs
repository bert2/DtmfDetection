namespace DtmfDetection.NAudio {
    using System;
    using global::NAudio.Wave;

    /// <summary>Provides an extension method to convert a byte size to a duration based on a wave format.</summary>
    public static class WaveFormatExt {
        /// <summary>Gets the latency in milliseconds equivalent to the size of a wave buffer.</summary>
        /// <param name="waveFormat">The format of the wave buffer.</param>
        /// <param name="bytes">The size of the buffer in bytes.</param>
        /// <returns>The latency in milliseconds.</returns>
        public static int ConvertByteSizeToLatency(this WaveFormat waveFormat, int bytes)
            => (int)Math.Round(1000 / ((double)waveFormat.AverageBytesPerSecond / bytes));
    }
}

namespace DtmfDetection.NAudio {
    using System;

    using global::NAudio.Utils;
    using global::NAudio.Wave;

    /// <summary>Decorates an `ISampleProvider` with a mono-conversion step.</summary>
    public class MonoSampleProvider : ISampleProvider {
        private readonly ISampleProvider sourceProvider;

        private readonly int sourceChannels;

        private float[] sourceBuffer = Array.Empty<float>();

        /// <summary>Creates a new `MonoSampleProvider` from a multi-channel `ISampleProvider`.</summary>
        /// <param name="sourceProvider">The `ISampleProvider` providing the source samples.</param>
        public MonoSampleProvider(ISampleProvider sourceProvider) {
            this.sourceProvider = sourceProvider;
            sourceChannels = sourceProvider.WaveFormat.Channels;
            WaveFormat = new WaveFormat(
                sourceProvider.WaveFormat.SampleRate,
                sourceProvider.WaveFormat.BitsPerSample,
                channels: 1);
        }

        /// <summary>The `WaveFormat` of the decorated `ISampleProvider`. Will match match the `WaveFormat` of the input `ISampleProvider` except that it will be mono (`WaveFormat.Channels` = 1).</summary>
        public WaveFormat WaveFormat { get; }

        /// <summary>Tries to read `count` sample frames from the input `ISampleProvider`, averages the sample values across all channels and writes one mixed sample value for each sample frame into `buffer`.</summary>
        /// <param name="buffer">The buffer to fill with samples.</param>
        /// <param name="offset">The offset into `buffer`.</param>
        /// <param name="count">The number of sample frames to read.</param>
        /// <returns>The number of samples written to the buffer.</returns>
        public int Read(float[] buffer, int offset, int count) {
            var sourceBytesRequired = count * sourceChannels;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);

            var samplesRead = sourceProvider.Read(sourceBuffer, offset * sourceChannels, sourceBytesRequired);
            var sampleFramesRead = samplesRead / sourceChannels;

            for (var sampleIndex = 0; sampleIndex < samplesRead; sampleIndex += sourceChannels)
                buffer[offset++] = Clamp(AverageAt(sampleIndex));

            return sampleFramesRead;
        }

        private float AverageAt(int frameStart) {
            var mixedValue = 0.0f;

            for (var channel = 0; channel < sourceChannels; channel++)
                mixedValue += sourceBuffer[frameStart + channel];

            return mixedValue / sourceChannels;
        }

        private static float Clamp(float value) => Math.Clamp(value, -1.0f, 1.0f);
    }
}

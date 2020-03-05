namespace DtmfDetection {
    using System;
    using DtmfDetection.Interfaces;

    /// <summary>Convenience implementation of `ISamples` for PCM audio data. PCM data is usually represented as an array of `float`s. The `Analyzer` uses `ISamples` to read the input data in blocks and feed them to the `Detector`.</summary>
    public class AudioData : ISamples {
        private readonly float[] samples;
        private long position;

        /// <summary>Returns the number of channels this `AudioData` has been created with.</summary>
        public int Channels { get; }

        /// <summary>Returns the sample rate this `AudioData` has been created with.</summary>
        public int SampleRate { get; }

        /// <summary>Calculates and returns the current position in the PCM data.</summary>
        public TimeSpan Position => new TimeSpan((long)Math.Round(position / Channels * 1000.0 / SampleRate));

        /// <summary>Creates a new `AudioData` from the given array of `float` values which were sampled with the given sample rate and for the given number of channels.</summary>
        /// <param name="samples">An array of `float`s representing the PCM data.</param>
        /// <param name="channels">The number of channels in the PCM data. If this value is greater than `1` then the sample values in `samples` must be interleaved (i.e. `left sample 1, right sample 1, left sample 2, right sample 2, ...`).</param>
        /// <param name="sampleRate">The sample rate of the PCM data in Hz. This should match the sample rate the `Analyzer` is using (via `Config.SampleRate`).</param>
        public AudioData(float[] samples, int channels, int sampleRate)
            => (this.samples, Channels, SampleRate) = (samples, channels, sampleRate);

        /// <summary>Reads `count` samples from the input and writes them into `buffer`. Because the input PCM data already has the expected format, this boils down to a simple call to `Array.Copy()`.</summary>
        /// <param name="buffer">The output array to write the read samples to.</param>
        /// <param name="count">The number of samples to read.</param>
        /// <returns>The number of samples that have been read. Will always equal `count` except when the end of the input has been reached, in which case `Read()` returns a number less than `count`.</returns>
        public int Read(float[] buffer, int count) {
            var safeCount = (int)Math.Min(count, samples.LongLength - position);
            Array.Copy(samples, position, buffer, 0, safeCount);
            position += safeCount;
            return safeCount;
        }
    }
}

namespace DtmfDetection.NAudio {
    using System;
    using DtmfDetection.Interfaces;
    using global::NAudio.Wave;

    /// <summary>Convenience implementation of `ISamples` for `WaveStream` audio. `WaveStream`s are commonly used for finite data
    /// like audio files.
    /// The `Analyzer` uses `ISamples` to read the input data in blocks and feed them to the `Detector`.</summary>
    public class AudioFile : ISamples {
        private readonly WaveStream source;
        private readonly ISampleProvider samples;

        /// <summary>Returns the number of channels in the `WaveStream` input or `1` when mono-conversion has been enabled.</summary>
        public int Channels => samples.WaveFormat.Channels;

        /// <summary>Returns the target sample rate this `AudioFile` has been created with.</summary>
        public int SampleRate => samples.WaveFormat.SampleRate;

        /// <summary>Returns the current position of the input `WaveStream`.</summary>
        public TimeSpan Position => source.CurrentTime;

        /// <summary>Creates a new `AudioFile` from a `WaveStream` input.
        /// Also resamples the input and optionally converts it to single-channel audio.</summary>
        /// <param name="source">The input audio data (typically finite).</param>
        /// <param name="targetSampleRate">Used to resample the `WaveStream` input. This should match the sample rate (in Hz) the `Analyzer` is
        /// using (via `Config.SampleRate`).</param>
        /// <param name="forceMono">Toggles conversion of multi-channel audio to mono.</param>
        public AudioFile(WaveStream source, int targetSampleRate, bool forceMono = true) {
            this.source = source;

            var samples = forceMono ? source.ToSampleProvider().AsMono() : source.ToSampleProvider();
            this.samples = samples.Resample(targetSampleRate);
        }

        /// <summary>Reads `count` samples from the input and writes them into `buffer`.</summary>
        /// <param name="buffer">The output array to write the read samples to.</param>
        /// <param name="count">The number of samples to read.</param>
        /// <returns>The number of samples that have been read. Will always equal `count` except when the end of the input has been reached,
        /// in which case `Read()` returns a number less than `count`.</returns>
        public int Read(float[] buffer, int count) => samples.Read(buffer, 0, count);
    }
}

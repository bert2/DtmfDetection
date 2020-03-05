namespace DtmfDetection.NAudio {
    using System;
    using DtmfDetection.Interfaces;
    using global::NAudio.Wave;

    /// <summary>Convenience implementation of `ISamples` for an infinite `IWaveIn` audio stream. The `Analyzer` uses `ISamples` to read the input data in blocks and feed them to the `Detector`.</summary>
    public class AudioStream : ISamples {
        private readonly BufferedWaveProvider source;
        private readonly ISampleProvider samples;
        private volatile bool stopRequested;

        /// <summary>Returns the number of channels in the `IWaveIn` input or `1` when mono-conversion has been enabled.</summary>
        public int Channels => samples.WaveFormat.Channels;

        /// <summary>Returns the target sample rate this `AudioStream` has been created with.</summary>
        public int SampleRate => samples.WaveFormat.SampleRate;

        /// <summary>Simply calls `DateTime.Now.TimeOfDay` and returns the result.</summary>
        public TimeSpan Position => DateTime.Now.TimeOfDay;

        /// <summary>Creates a new `AudioStream` from an `IWaveIn` input by buffering it with a `BufferedWaveProvider`. Also resamples the input and optionally converts it to single-channel audio.</summary>
        /// <param name="source">The infinite input audio stream.</param>
        /// <param name="targetSampleRate">Used to resample the `IWaveIn` input. This should match the sample rate (in Hz) the `Analyzer` is using (via `Config.SampleRate`).</param>
        /// <param name="forceMono">Toggles conversion of multi-channel audio to mono.</param>
        public AudioStream(IWaveIn source, int targetSampleRate, bool forceMono = true) {
            this.source = source.ToBufferedWaveProvider();
            var samples = forceMono ? this.source.ToSampleProvider().AsMono() : this.source.ToSampleProvider();
            this.samples = samples.Resample(targetSampleRate);
        }

        /// <summary>Reads `count` samples from the input and writes them into `buffer`. Will block as long as it takes for the input to buffer the requested number of samples.</summary>
        /// <param name="buffer">The output array to write the read samples to.</param>
        /// <param name="count">The number of samples to read.</param>
        /// <returns>The number of samples that have been read. Will always equal `count` except when `StopWaiting()` has been called,in which case `Read()` returns `0`.</returns>
        public int Read(float[] buffer, int count) {
            while (source.WaitForSamples(count)) {
                if (stopRequested) return 0;
            }

            return samples.Read(buffer, 0, count);
        }

        /// <summary>Stops waiting for the input to buffer data. Some `IWaveIn`s don't have data available continuously. For instance a `WasapiLoopbackCapture` will only have data as long as the OS is playing some audio. Calling `StopWaiting()` will break the infinite wait loop and the `Analyzer` processing this `AudioStream` will consider it being "finished". This in turn helps to gracefully exit the thread running the analysis.</summary>
        public void StopWaiting() => stopRequested = true;
    }
}

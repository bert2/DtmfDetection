namespace DtmfDetection.Interfaces {
    using System;
    using System.Collections.Generic;

    /// <summary>Interface to decouple the `Analyzer` from the `Detector` it is using by default. Use this if you want to inject your own detector into the `Analyzer`. Feel free to start by copying the original `Detector` and adjust it to your needs.</summary>
    public interface IDetector {
        /// <summary>The number of channels this detector has been created for. Used by the `Analyzer` to validate that this detector supports the number of channels present int the source data (`ISamples.Channels`).</summary>
        int Channels { get; }

        /// <summary>The `Config` this detector has been created with.</summary>
        Config Config { get; }

        /// <summary>Runs the Goertzel algorithm on all samples in `sampleBlock` and returns the DTMF key detected in each channel. `PhoneKey.None` is used in case no DTMF key has been detected in a channel.</summary>
        /// <param name="sampleBlock">The block of samples to analyze. Its length should always match `Config.SampleBlockSize` except when the end of the input has been reached, in which case it might be smalller once.</param>
        /// <returns>A list of DTMF keys, one for each channel. Hence its length must match the value of `Channels`.</returns>
        IReadOnlyList<PhoneKey> Detect(in ReadOnlySpan<float> sampleBlock);
    }
}

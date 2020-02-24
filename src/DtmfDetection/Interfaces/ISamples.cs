namespace DtmfDetection.Interfaces {
    using System;

    /// <summary>Interface used by the `Analyzer` to access a variety of audio sources in a uniform way. Implement this interface if
    /// your data source does not fit any of the pre-built implementations:
    ///   `AudioData` for float arrays of PCM data,
    ///   `AudioFile` for audio files, or
    ///   `AudioStream` for infinite audio streams.</summary>
    public interface ISamples {
        /// <summary>The number of audio channels. This should match the value of `Config.Channels` the `Analyzer` is using.</summary>
        int Channels { get; }

        /// <summary>The rate at which the values have been sampled in Hz. This should match the value of `Config.SampleRate` the `Analyzer`
        /// is using.</summary>
        int SampleRate { get; }

        /// <summary>The position of the "read cursor" in the sample stream. This should increase with every call to `Read()` that returns a
        /// value greater than 0.</summary>
        TimeSpan Position { get; }

        /// <summary>Reads `count` samples from the input and writes them into `buffer`. This method should either block until `count` samples
        /// have been succesfully read, or return a number less than `count` to indicate that the end of the stream has been reached. Once
        /// the end of stream has been reached, subsequent calls to `Read()` are allowed to fail with exceptions.</summary>
        /// <param name="buffer">The output array to write the read samples to.</param>
        /// <param name="count">The number of samples to read.</param>
        /// <returns>The number of samples that have been read. Will always equal `count` except when the end of the input has been reached,
        /// in which case `Read()` returns a number less than `count`.</returns>
        int Read(float[] buffer, int count);
    }
}

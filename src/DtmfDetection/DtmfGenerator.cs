namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection.Interfaces;

    /// <summary>Provides helpers to generate DTMF tones.</summary>
    public static class DtmfGenerator {
        /// <summary>Generates single-channel PCM data playing the DTMF tone `key` infinitely.</summary>
        /// <param name="key">The DTMF tone to generate.</param>
        /// <param name="sampleRate">Optional sample rate of the PCM data. Defaults to `Config.DefaultSampleRate`.</param>
        /// <returns>An infinite sequence of PCM data playing the specified DMTF tone.</returns>
        public static IEnumerable<float> Generate(PhoneKey key, int sampleRate = Config.DefaultSampleRate)
            => Generate(key.ToDtmfTone(), sampleRate);

        /// <summary>Generates single-channel PCM data playing the dual tone comprised of the two frequencies `highFreq` and `lowFreq` infinitely.</summary>
        /// <param name="highFreq">The high frequency part of the dual tone.</param>
        /// <param name="lowFreq">The low frequency part of the dual tone.</param>
        /// <param name="sampleRate">Optional sample rate of the PCM data. Defaults to `Config.DefaultSampleRate`.</param>
        /// <returns>An infinite sequence of PCM data playing the specified dual tone.</returns>
        public static IEnumerable<float> Generate(int highFreq, int lowFreq, int sampleRate = Config.DefaultSampleRate)
            => Sine(highFreq, sampleRate).Add(Sine(lowFreq, sampleRate)).Normalize(1);

        /// <summary>Generates single-channel PCM data playing the dual tone comprised of the two frequencies `highFreq` and `lowFreq` infinitely.</summary>
        /// <param name="dual">A tuple holding the high and low frequency.</param>
        /// <param name="sampleRate">Optional sample rate of the PCM data. Defaults to `Config.DefaultSampleRate`.</param>
        /// <returns>An infinite sequence of PCM data playing the specified dual tone.</returns>
        public static IEnumerable<float> Generate((int highFreq, int lowFreq) dual, int sampleRate = Config.DefaultSampleRate)
            => Generate(dual.highFreq, dual.lowFreq, sampleRate);

        /// <summary>Generates single-channel PCM data playing the DTMF tone `key` for the specified length `ms`.</summary>
        /// <param name="key">The DTMF tone to generate.</param>
        /// <param name="ms">The length of the DTMF tone in milliseconds.</param>
        /// <param name="sampleRate">Optional sample rate of the PCM data. Defaults to `Config.DefaultSampleRate`.</param>
        /// <returns>A sequence of PCM data playing the specified DTMF tone.</returns>
        public static IEnumerable<float> Mark(PhoneKey key, int ms = 40, int sampleRate = Config.DefaultSampleRate)
            => Generate(key, sampleRate).Take(NumSamples(ms, channels: 1, sampleRate));

        /// <summary>Generates single-channel PCM data playing silence for the specified length `ms`.</summary>
        /// <param name="ms">The length of the silence in milliseconds.</param>
        /// <param name="sampleRate">Optional sample rate of the PCM data. Defaults to `Config.DefaultSampleRate`.</param>
        /// <returns>A sequence of silent PCM data.</returns>
        public static IEnumerable<float> Space(int ms = 20, int sampleRate = Config.DefaultSampleRate)
            => Constant(.0f).Take(NumSamples(ms, channels: 1, sampleRate));

        /// <summary>Takes two sequences of single-channel PCM data and interleaves them to form a single sequence of dual-channel PCM data.</summary>
        /// <param name="left">The PCM data for the left channel.</param>
        /// <param name="right">The PCM data for the right channel.</param>
        /// <returns>A sequence of dual-channel PCM data.</returns>
        public static IEnumerable<float> Stereo(IEnumerable<float> left, IEnumerable<float> right)
            => left.Zip(right, (l, r) => new[] { l, r }).SelectMany(x => x);

        /// <summary>Generates a sinusoidal PCM signal of infinite length for the specified frequency.</summary>
        /// <param name="freq">The frequency of the signal.</param>
        /// <param name="sampleRate">Optional sample rate of the PCM data. Defaults to `Config.DefaultSampleRate`.</param>
        /// <param name="amplitude">Optional amplitude of the signal. Defaults to `1`.</param>
        /// <returns>An infinite sine signal.</returns>
        public static IEnumerable<float> Sine(int freq, int sampleRate = Config.DefaultSampleRate, float amplitude = 1) {
            for (var t = 0.0; ; t += 1.0 / sampleRate)
                yield return (float)(amplitude * Math.Sin(2.0 * Math.PI * freq * t));
        }

        /// <summary>Generates a constant PCM signal of infinite length.</summary>
        /// <param name="amplitude">The amplitude of the signal.</param>
        /// <returns>An infinite constant signal.</returns>
        public static IEnumerable<float> Constant(float amplitude) {
            while (true) yield return amplitude;
        }

        /// <summary>Generates an infinite PCM signal of pseudo-random white noise.</summary>
        /// <param name="amplitude">The amplitude of the noise.</param>
        /// <returns>An infinite noise signal.</returns>
        public static IEnumerable<float> Noise(float amplitude) {
            var rng = new Random();
            while (true) {
                var n = amplitude * rng.NextDouble();
                var sign = Math.Pow(-1, rng.Next(2));
                yield return (float)(sign * n);
            }
        }

        /// <summary>Creates an `AudioData` instance from a sequence of PCM samples.</summary>
        /// <param name="source">The input PCM data.</param>
        /// <param name="channels">Optional number of channels in the PCM data. Defaults to `1`.</param>
        /// <param name="sampleRate">Optional sample rate of the PCM data. Defaults to `Config.DefaultSampleRate`.</param>
        /// <returns>A new `AudioData` instane that can be analyzed by the `Analyzer`.</returns>
        public static ISamples AsSamples(
            this IEnumerable<float> source,
            int channels = 1,
            int sampleRate = Config.DefaultSampleRate)
            => new AudioData(source.ToArray(), channels, sampleRate);

        /// <summary>Adds two sequences of PCM data together. Used to generate dual tones. The amplitude might exceed the range `[-1..1]` after adding.</summary>
        /// <param name="xs">One of the two input signals to add.</param>
        /// <param name="ys">One of the two input signals to add.</param>
        /// <returns>The sum of both input signals.</returns>
        public static IEnumerable<float> Add(this IEnumerable<float> xs, IEnumerable<float> ys)
            => xs.Zip(ys, (l, r) => l + r);

        /// <summary>Normlizes a signal with the given `maxAmplitude`.</summary>
        /// <param name="source">The signal to normalize.</param>
        /// <param name="maxAmplitude">The value to normalize by. Ideally it should equal `Math.Abs(source.Max())`.</param>
        /// <returns>The input signal with each sample value divided by `maxAmplitude`.</returns>
        public static IEnumerable<float> Normalize(this IEnumerable<float> source, float maxAmplitude)
            => source.Select(x => x / maxAmplitude);

        /// <summary>Concatenates multiple finite sequences of PCM data. Typically used with `Mark()` and `Space()`.</summary>
        /// <param name="xss">The sequences to concatenate.</param>
        /// <returns>The single sequence that is the concatenation of the given sequences.</returns>
        public static IEnumerable<float> Concat(params IEnumerable<float>[] xss) => xss.SelectMany(xs => xs);

        /// <summary>Converts a duration in milliseconds into the number of samples required to represent a signal of that duration as PCM audio data.</summary>
        /// <param name="milliSeconds">The duration of the signal.</param>
        /// <param name="channels">Optional number of channels in the signal. Defaults to `1`.</param>
        /// <param name="sampleRate">Optional sample rate of the signal. Defaults to `Config.DefaultSampleRate`.</param>
        /// <returns>The number of samples needed for the specified length, channels, and sample rate.</returns>
        public static int NumSamples(int milliSeconds, int channels = 1, int sampleRate = Config.DefaultSampleRate)
            => channels * (int)Math.Round(milliSeconds / (1.0 / sampleRate * 1000));
    }
}

namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection.Interfaces;

    public static class DtmfGenerator {
        public static IEnumerable<float> Generate(PhoneKey k, int sampleRate = Config.DefaultSampleRate)
            => Generate(k.ToDtmfTone(), sampleRate);

        public static IEnumerable<float> Generate(int highFreq, int lowFreq, int sampleRate = Config.DefaultSampleRate)
            => Sine(highFreq, sampleRate).Add(Sine(lowFreq, sampleRate)).Normalize(1);

        public static IEnumerable<float> Generate((int highFreq, int lowFreq) dual, int sampleRate = Config.DefaultSampleRate)
            => Generate(dual.highFreq, dual.lowFreq, sampleRate);

        public static IEnumerable<float> Mark(PhoneKey k, int ms = 40, int sampleRate = Config.DefaultSampleRate)
            => Generate(k, sampleRate).Take(NumSamples(ms, channels: 1, sampleRate));

        public static IEnumerable<float> Space(int ms = 20, int sampleRate = Config.DefaultSampleRate)
            => Constant(.0f).Take(NumSamples(ms, channels: 1, sampleRate));

        public static IEnumerable<T> Stereo<T>(IEnumerable<T> left, IEnumerable<T> right)
            => left.Zip(right, (l, r) => new[] { l, r }).SelectMany(x => x);

        public static IEnumerable<float> Sine(int freq, int sampleRate = Config.DefaultSampleRate, float amplitude = 1) {
            for (var t = 0.0; ; t += 1.0 / sampleRate)
                yield return (float)(amplitude * Math.Sin(2.0 * Math.PI * freq * t));
        }

        public static IEnumerable<float> Constant(float amplitude) {
            while (true) yield return amplitude;
        }

        public static IEnumerable<float> Noise(float amplitude) {
            var rng = new Random();
            while (true) {
                var n = amplitude * rng.NextDouble();
                var sign = Math.Pow(-1, rng.Next(2));
                yield return (float)(sign * n);
            }
        }

        public static ISamples AsSamples(
            this IEnumerable<float> source,
            int channels = 1,
            int sampleRate = Config.DefaultSampleRate)
            => new AudioData(source.ToArray(), channels, sampleRate);

        public static IEnumerable<float> Add(this IEnumerable<float> left, IEnumerable<float> right)
            => left.Zip(right, (l, r) => l + r);

        public static IEnumerable<float> Normalize(this IEnumerable<float> source, float maxAmplitude)
            => source.Select(x => x / maxAmplitude);

        public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] xs) => xs.SelectMany(x => x);

        public static int NumSamples(int milliSeconds, int channels = 1, int sampleRate = Config.DefaultSampleRate)
            => channels * (int)Math.Round(milliSeconds / (1.0 / sampleRate * 1000));
    }
}

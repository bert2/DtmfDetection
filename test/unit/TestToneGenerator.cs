namespace Unit {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;
    using MoreLinq;

    public static class TestToneGenerator {
        public const int DefaultSampleRate = 8000;

        public static IEnumerable<float> Sine(int freq, float amplitude = 1.0f, int sampleRate = DefaultSampleRate) {
            var t = 0.0;
            var dt = 1.0 / sampleRate;

            while (true) {
                yield return (float)(amplitude * Math.Sin(2.0 * Math.PI * freq * t));
                t += dt;
            }
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

        public static IEnumerable<float> ToDual(in this (int f1, int f2) tones)
            => Sine(tones.f1).Add(Sine(tones.f2)).Norm(2);

        public static IEnumerable<float> DtmfTone(PhoneKey k) => k.ToDtmfTone().ToDual();

        public static float[] DtmfToneBlock(PhoneKey k) => DtmfTone(k).FirstBlock();

        public static float[] FirstBlock(this IEnumerable<float> samples, int numChannels = 1, int blockSize = 205)
            => samples.Take(blockSize * numChannels).ToArray();

        public static IEnumerable<float> Add(
            this IEnumerable<float> left,
            IEnumerable<float> right)
            => left.Combine(right, (l, r) => l + r);

        public static IEnumerable<float> Combine(
            this IEnumerable<float> left,
            IEnumerable<float> right,
            Func<float, float, float> f)
            => left.Zip(right, (l, r) => f(l, r));

        public static IEnumerable<float> Norm(this IEnumerable<float> source, float maxAmplitude)
            => source.Select(x => x / maxAmplitude);

        public static ISamples AsSamples(this IEnumerable<float> source) => new Samples(source);
    }

    public class Samples : ISamples {
        private readonly float[] samples;
        private int position;
        public Samples(IEnumerable<float> samples) => this.samples = samples.ToArray();

        public int Read(float[] buffer, int count) {
            var safeCount = Math.Min(count, samples.Length - position);
            Array.Copy(samples, position, buffer, 0, safeCount);
            position += safeCount;
            return safeCount;
        }
    }
}

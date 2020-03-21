namespace Unit {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;
    using Shouldly;
    using Xunit;

    using static DtmfDetection.DtmfGenerator;

    public class GoertzelTests {
        [Theory]
        [InlineData(697)]
        [InlineData(770)]
        [InlineData(852)]
        [InlineData(941)]
        public void CanDetectAllLowTones(int freq) =>
            Sine(freq)
            .MeasureFrequency(freq)
            .GoertzelResponseShouldBeGreaterThan(Config.Default.Threshold);

        [Theory]
        [InlineData(1209)]
        [InlineData(1336)]
        [InlineData(1477)]
        [InlineData(1633)]
        public void CanDetectAllHighTones(int freq) =>
            Sine(freq)
            .MeasureFrequency(freq)
            .GoertzelResponseShouldBeGreaterThan(Config.Default.Threshold);

        [Fact]
        public void DoesNotDetectFrequencyInConstantSignal() =>
            Constant(.3f)
            .MeasureFrequency(1209)
            .GoertzelResponseShouldBeLessThan(Config.Default.Threshold);

        [Fact]
        public void CanDetectWeakFrequency() =>
            Sine(1209, amplitude: .1f)
            .MeasureFrequency(1209)
            .GoertzelResponseShouldBeGreaterThan(Config.Default.Threshold);

        [Fact]
        public void CanDetectFrequencyInNoisySignal() => Repeat(() =>
            Sine(1336).Add(Noise(.1f)).Normalize(1.1f)
            .MeasureFrequency(1336)
            .GoertzelResponseShouldBeGreaterThan(Config.Default.Threshold));

        [Fact]
        public void CanDetectFrequencyInVeryNoisySignal() => Repeat(() =>
            Sine(941).Add(Noise(.5f)).Normalize(1.5f)
            .MeasureFrequency(941)
            .GoertzelResponseShouldBeGreaterThan(Config.Default.Threshold));

        [Fact]
        public void CanDetectFrequencyInOverlappingFrequency() =>
            Sine(697).Add(Sine(1633)).Normalize(2)
            .MeasureFrequency(697)
            .GoertzelResponseShouldBeGreaterThan(Config.Default.Threshold);

        [Fact]
        public void CanDetectWeakFrequencyInOverlappingFrequencyAndNoisySignal() => Repeat(() =>
            Sine(1477, amplitude: .4f).Add(Sine(852, amplitude: .4f)).Add(Noise(.1f))
            .MeasureFrequency(1477)
            .GoertzelResponseShouldBeGreaterThan(Config.Default.Threshold));

        [Fact]
        public void CanDetectFrequencyThatStartsLate() =>
            Constant(.0f).Take(102).Concat(Sine(1336))
            .MeasureFrequency(1336)
            .GoertzelResponseShouldBeGreaterThan(Config.Default.Threshold);

        [Fact]
        public void CanBeResetted() =>
            Sine(1336).MeasureFrequency(1336).Reset()
            .Response
            .ShouldBe(0);

        [Fact]
        public void AlsoResetsTotalEnergy() =>
            Sine(1336).MeasureFrequency(1336).Reset()
            .NormResponse
            .ShouldBe(double.NaN);

        [Fact]
        public void ToStringPrintsNormalizedResponse() =>
            new Goertzel(1, 2, 3, 4)
            .ToString()
            .ShouldBe("1.75");

        #region Equality implementations

        [Fact]
        public void ImplementsIEquatable() =>
            new HashSet<Goertzel> { new Goertzel(1, 2, 3, 4) }
            .Contains(new Goertzel(1, 2, 3, 4))
            .ShouldBeTrue();

        [Fact]
        public void OverridesGetHashCode() =>
            new Goertzel(1, 2, 3, 4).GetHashCode()
            .ShouldNotBe(new Goertzel(5, 6, 7, 8).GetHashCode());

        [Fact]
        public void OverridesEquals() =>
            new Goertzel(1, 2, 3, 4)
            .Equals((object)new Goertzel(1, 2, 3, 4))
            .ShouldBeTrue();

        [Fact]
        public void OverridesEqualsOperator() =>
            (new Goertzel(1, 2, 3, 4) == new Goertzel(1, 2, 3, 4))
            .ShouldBeTrue();

        [Fact]
        public void OverridesNotEqualsOperator() =>
            (new Goertzel(1, 2, 3, 4) != new Goertzel(5, 6, 7, 8))
            .ShouldBeTrue();

        #endregion Equality implementations

        private static void Repeat(Action test, int count = 1000) {
            for (var i = 1; i <= count; i++) {
                try {
                    test();
                } catch (ShouldAssertException e) {
                    throw new ShouldAssertException($"Failed at try #{i}:", e);
                }
            }
        }
    }

    public static class GoertzelTestsExt {
        public static Goertzel MeasureFrequency(this IEnumerable<float> samples, int targetFreq) => samples
            .Take(Config.Default.SampleBlockSize)
            .Aggregate(
                Goertzel.Init(targetFreq, Config.Default.SampleRate, Config.Default.SampleBlockSize),
                (g, s) => g.AddSample(s));

        public static void GoertzelResponseShouldBeGreaterThan(this in Goertzel g, double threshold)
            => g.NormResponse.ShouldBeGreaterThan(threshold, g.ToMessage());

        public static void GoertzelResponseShouldBeLessThan(this in Goertzel g, double threshold)
            => g.NormResponse.ShouldBeLessThan(threshold, g.ToMessage());

        private static string ToMessage(this in Goertzel g) =>
            $"response:\t\t\t\t{g.Response, 8:#.###}\n"
            + $"\tnormalized response:\t{g.NormResponse, 8:#.###}\n"
            + $"\ttotal signal energy:\t{g.Response / g.NormResponse, 8:#.###}";
    }
}

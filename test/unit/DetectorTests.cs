namespace Unit {
    using System;
    using DtmfDetection;
    using MoreLinq;
    using Shouldly;
    using Xunit;

    using static DtmfDetection.Utils;
    using static TestToneGenerator;

    public class DetectorTests {
        [Fact]
        public void DetectsAllPhoneKeys() => PhoneKeys().ForEach(key =>
            DtmfToneBlock(key)
            .Analyze()
            .ShouldBe(new[] { key }));

        [Fact]
        public void CanBeReused() =>
            new Detector().With(d => _ = d.Analyze(DtmfToneBlock(PhoneKey.Three)))
            .Analyze(DtmfToneBlock(PhoneKey.C))
            .ShouldBe(new[] { PhoneKey.C });

        [Fact]
        public void SupportsStereo() =>
            DtmfTone(PhoneKey.One).Interleave(DtmfTone(PhoneKey.Two)).FirstBlock(numChannels: 2)
            .Analyze(numChannels: 2)
            .ShouldBe(new[] { PhoneKey.One, PhoneKey.Two });

        [Fact]
        public void SupportsQuadChannel() =>
            DtmfTone(PhoneKey.One).Interleave(DtmfTone(PhoneKey.Two), DtmfTone(PhoneKey.Three), DtmfTone(PhoneKey.Four)).FirstBlock(numChannels: 4)
            .Analyze(numChannels: 4)
            .ShouldBe(new[] { PhoneKey.One, PhoneKey.Two, PhoneKey.Three, PhoneKey.Four });
    }

    public static class DetectorTestsExt {
        public static object Analyze(this float[] samples, int numChannels = 1) => new Detector(numChannels).Analyze(samples);

        public static T With<T>(this T x, Action<T> action) { action?.Invoke(x); return x; }
    }
}

namespace Unit {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;
    using MoreLinq;
    using Shouldly;
    using Xunit;

    using static DtmfDetection.DtmfGenerator;
    using static DtmfDetection.Utils;
    using static DetectorTestsExt;

    public class DetectorTests {
        [Fact]
        public void DetectsAllPhoneKeys() => PhoneKeys().ForEach(key =>
            DtmfToneBlock(key)
            .Analyze()
            .ShouldBe(new[] { key }));

        [Fact]
        public void CanBeReused() =>
            new Detector(1, Config.Default).With(d => _ = d.Detect(DtmfToneBlock(PhoneKey.Three)))
            .Detect(DtmfToneBlock(PhoneKey.C))
            .ShouldBe(new[] { PhoneKey.C });

        [Fact]
        public void SupportsStereo() =>
            Generate(PhoneKey.One).Interleave(Generate(PhoneKey.Two)).FirstBlock(channels: 2)
            .Analyze(channels: 2)
            .ShouldBe(new[] { PhoneKey.One, PhoneKey.Two });

        [Fact]
        public void SupportsQuadChannel() =>
            Generate(PhoneKey.One).Interleave(Generate(PhoneKey.Two), Generate(PhoneKey.Three), Generate(PhoneKey.Four)).FirstBlock(channels: 4)
            .Analyze(channels: 4)
            .ShouldBe(new[] { PhoneKey.One, PhoneKey.Two, PhoneKey.Three, PhoneKey.Four });
    }

    public static class DetectorTestsExt {
        public static object Analyze(this float[] samples, int channels = 1)
            => new Detector(channels, Config.Default).Detect(samples);

        public static T With<T>(this T x, Action<T> action) { action?.Invoke(x); return x; }

        public static float[] DtmfToneBlock(PhoneKey k) => Generate(k).FirstBlock();

        public static float[] FirstBlock(this IEnumerable<float> samples, int channels = 1, int blockSize = Config.DefaultSampleBlockSize)
            => samples.Take(blockSize * channels).ToArray();
    }
}

namespace Unit {
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;
    using MoreLinq;
    using Shouldly;
    using Xunit;

    using static TestToneGenerator;
    using static SampleProcessorTestsExt;

    public class SampleProcessorTests {
        [Fact]
        public void ReturnsStartAndStopOfSingleTone() =>
            Mark(PhoneKey.Five)
            .Process()
            .ShouldBe(new[] { DtmfChange.Start(PhoneKey.Five, 0), DtmfChange.Stop(PhoneKey.Five, 0) });

        [Fact]
        public void ReturnsStartAndStopOfMultipleTones() =>
            Concat(Space(), Mark(PhoneKey.A), Space(), Mark(PhoneKey.C), Space(), Mark(PhoneKey.A), Space(), Mark(PhoneKey.B), Space())
            .Process()
            .ShouldBe(new[] {
                DtmfChange.Start(PhoneKey.A, 0), DtmfChange.Stop(PhoneKey.A, 0),
                DtmfChange.Start(PhoneKey.C, 0), DtmfChange.Stop(PhoneKey.C, 0),
                DtmfChange.Start(PhoneKey.A, 0), DtmfChange.Stop(PhoneKey.A, 0),
                DtmfChange.Start(PhoneKey.B, 0), DtmfChange.Stop(PhoneKey.B, 0)
            });

        [Fact]
        public void ReturnsStartAndStopOfMultipleOverlappingStereoTones() =>
            Stereo(
                left:  Concat(Mark(PhoneKey.A, ms: 80), Space(ms: 40), Mark(PhoneKey.C, ms: 80), Space(ms: 40)),
                right: Concat(Space(ms: 40), Mark(PhoneKey.B, ms: 80), Space(ms: 40), Mark(PhoneKey.D, ms: 80)))
            .Process(numChannels: 2)
            .ShouldBe(new[] {
                // left channel (0)              right channel (1)
                DtmfChange.Start(PhoneKey.A, 0),
                                                 DtmfChange.Start(PhoneKey.B, 1),
                DtmfChange.Stop(PhoneKey.A, 0),
                                                 DtmfChange.Stop(PhoneKey.B, 1),
                DtmfChange.Start(PhoneKey.C, 0),
                                                 DtmfChange.Start(PhoneKey.D, 1),
                DtmfChange.Stop(PhoneKey.C, 0),
                                                 DtmfChange.Stop(PhoneKey.D, 1),
            });
    }

    public static class SampleProcessorTestsExt {
        public static List<DtmfChange> Process(this IEnumerable<float> samples, int numChannels = 1) {
            var p = new SampleProcessor(samples.AsSamples(), numChannels);

            var dtmfs = new List<DtmfChange>();
            while (p.CanRead) dtmfs.AddRange(p.ProcessNext());

            return dtmfs;
        }

        public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] xs) => xs.SelectMany(x => x);

        public static IEnumerable<T> Stereo<T>(IEnumerable<T> left, IEnumerable<T> right) => left.Interleave(right);
    }
}

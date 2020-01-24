namespace Unit {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;
    using MoreLinq;
    using Shouldly;
    using Xunit;

    using static TestToneGenerator;
    using static AnalyzerTestsExt;

    public class AnalyzerTests {
        [Fact]
        public void ReturnsStartAndStopOfSingleTone() =>
            Mark(PhoneKey.Five)
            .Process().AndIgnorePositions()
            .ShouldBe(new[] { Start(PhoneKey.Five), Stop(PhoneKey.Five) });

        [Fact]
        public void SignalsThatNoMoreDataIsAvailable() {
            var analyzer = Analyzer.Create(DtmfToneBlock(PhoneKey.Zero).AsSamples(), Config.Default);
            _ = analyzer.AnalyzeNextBlock();

            _ = analyzer.AnalyzeNextBlock();

            analyzer.MoreSamplesAvailable.ShouldBeFalse();
        }

        [Fact]
        public void ReturnsStopOfCutOffTones() =>
            Mark(PhoneKey.Eight, ms: 24)
            .Process().AndIgnorePositions()
            .ShouldBe(new[] { Start(PhoneKey.Eight), Stop(PhoneKey.Eight) });

        [Fact]
        public void ReturnsStartAndStopOfMultipleTones() =>
            Concat(Space(), Mark(PhoneKey.A), Space(), Mark(PhoneKey.C), Space(), Mark(PhoneKey.A), Space(), Mark(PhoneKey.B), Space())
            .Process().AndIgnorePositions()
            .ShouldBe(new[] {
                Start(PhoneKey.A), Stop(PhoneKey.A),
                Start(PhoneKey.C), Stop(PhoneKey.C),
                Start(PhoneKey.A), Stop(PhoneKey.A),
                Start(PhoneKey.B), Stop(PhoneKey.B)
            });

        [Fact]
        public void ReturnsStartAndStopOfMultipleOverlappingStereoTones() =>
            Stereo(
                left:  Concat(Mark(PhoneKey.A, ms: 80), Space(ms: 40), Mark(PhoneKey.C, ms: 80), Space(ms: 40)),
                right: Concat(Space(ms: 40), Mark(PhoneKey.B, ms: 80), Space(ms: 40), Mark(PhoneKey.D, ms: 80)))
            .Process(numChannels: 2).AndIgnorePositions()
            .ShouldBe(new[] {
                // left channel         // right channel
                Start(PhoneKey.A, 0),   Start(PhoneKey.B, 1),
                Stop(PhoneKey.A, 0),    Stop(PhoneKey.B, 1),
                Start(PhoneKey.C, 0),   Start(PhoneKey.D, 1),
                Stop(PhoneKey.C, 0),    Stop(PhoneKey.D, 1)
            });
    }

    public static class AnalyzerTestsExt {
        public static List<DtmfChange> Process(this IEnumerable<float> samples, int numChannels = 1) {
            var analyzer = Analyzer.Create(samples.AsSamples(numChannels), Config.Default);

            var dtmfs = new List<DtmfChange>();
            while (analyzer.MoreSamplesAvailable) dtmfs.AddRange(analyzer.AnalyzeNextBlock());

            return dtmfs;
        }

        public static IEnumerable<DtmfChange> AndIgnorePositions(this IEnumerable<DtmfChange> dtmfs) => dtmfs
            .Select(x => new DtmfChange(x.Key, new TimeSpan(), x.Channel, x.IsStart));

        public static DtmfChange Start(PhoneKey k, int channel = 0) => DtmfChange.Start(k, new TimeSpan(), channel);

        public static DtmfChange Stop(PhoneKey k, int channel = 0) => DtmfChange.Stop(k, new TimeSpan(), channel);

        public static TimeSpan MS(int milliSeconds) => TimeSpan.FromMilliseconds(milliSeconds);

        public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] xs) => xs.SelectMany(x => x);

        public static IEnumerable<T> Stereo<T>(IEnumerable<T> left, IEnumerable<T> right) => left.Interleave(right);
    }
}

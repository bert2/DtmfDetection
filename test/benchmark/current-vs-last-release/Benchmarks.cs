namespace Benchmark.CurrentVsLastRelease {
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using DtmfDetection;

    using static Unit.TestToneGenerator;

    [MemoryDiagnoser, NativeMemoryProfiler]
    public class Benchmarks {
        private readonly DtmfDetection.LastRelease.DtmfDetector lastRelease = new DtmfDetection.LastRelease.DtmfDetector(
            new DtmfDetection.LastRelease.DetectorConfig(),
            new[] {
                new DtmfDetection.LastRelease.PureTones(new DtmfDetection.LastRelease.AmplitudeEstimatorFactory(
                    new DtmfDetection.LastRelease.DetectorConfig().MaxSampleRate,
                    new DtmfDetection.LastRelease.DetectorConfig().SampleBlockSize))
            });

        private readonly Detector current = new Detector();

        private readonly float[] sampleBlock = DtmfToneBlock(PhoneKey.One);

        [Benchmark(Baseline = true)]
        public DtmfDetection.LastRelease.DtmfTone[] LastRelease() => lastRelease.Analyze(sampleBlock);

        [Benchmark]
        public object Current() => current.Analyze(sampleBlock);
    }
}

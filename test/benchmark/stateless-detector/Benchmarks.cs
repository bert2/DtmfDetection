namespace Benchmark.StatelessDetector {
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using DtmfDetection;

    using static Unit.TestToneGenerator;

    [MemoryDiagnoser, NativeMemoryProfiler]
    public class Benchmarks {
        private readonly Detector currentDetector = new Detector();
        private readonly StatefulDetector statefulDetector = new StatefulDetector();
        private readonly LessStatefulDetector lessStatefulDetector = new LessStatefulDetector();
        private readonly MuchLessStatefulDetector muchLessStatefulDetector = new MuchLessStatefulDetector();
        private readonly LinqedDetector linqedDetector = new LinqedDetector();

        private readonly float[] sampleBlock = DtmfToneBlock(PhoneKey.One);

        [Benchmark(Baseline = true)]
        public PhoneKey[] CurrentDetector() => currentDetector.Analyze(sampleBlock);

        public PhoneKey StatefulDetector() => statefulDetector.Analyze(sampleBlock);

        [Benchmark]
        public PhoneKey LessStatefulDetector() => lessStatefulDetector.Analyze(sampleBlock);

        [Benchmark]
        public PhoneKey MuchLessStatefulDetector() => muchLessStatefulDetector.Analyze(sampleBlock);

        [Benchmark]
        public PhoneKey LinqedDetector() => linqedDetector.Analyze(sampleBlock);
    }
}

namespace Benchmark.StatelessDetector {
    using System.Linq;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using DtmfDetection;

    using static DtmfDetection.DtmfGenerator;

    [MemoryDiagnoser, NativeMemoryProfiler]
    public class Benchmarks {
        private readonly Detector currentDetector = new Detector(channels: 1, Config.Default);
        private readonly StatefulDetector statefulDetector = new StatefulDetector();
        private readonly LessStatefulDetector lessStatefulDetector = new LessStatefulDetector();
        private readonly MuchLessStatefulDetector muchLessStatefulDetector = new MuchLessStatefulDetector();
        private readonly LinqedDetector linqedDetector = new LinqedDetector();

        private readonly float[] sampleBlock = Generate(PhoneKey.One).Take(Config.DefaultSampleBlockSize).ToArray();

        [Benchmark(Baseline = true)]
        public object CurrentDetector() => currentDetector.Detect(sampleBlock);

        public PhoneKey StatefulDetector() => statefulDetector.Analyze(sampleBlock);

        [Benchmark]
        public PhoneKey LessStatefulDetector() => lessStatefulDetector.Analyze(sampleBlock);

        [Benchmark]
        public PhoneKey MuchLessStatefulDetector() => muchLessStatefulDetector.Analyze(sampleBlock);

        [Benchmark]
        public PhoneKey LinqedDetector() => linqedDetector.Analyze(sampleBlock);
    }
}

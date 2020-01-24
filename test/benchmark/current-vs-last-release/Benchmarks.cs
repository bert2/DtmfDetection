namespace Benchmark.CurrentVsLastRelease {
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnostics.Windows.Configs;
    using DtmfDetection;
    using DtmfDetection.NAudio;
    using DtmfDetection.NAudio.LastRelease;
    using NAudio.Wave;

    [MemoryDiagnoser, NativeMemoryProfiler]
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable")]
    public class Benchmarks {
        private readonly AudioFileReader audioFile = new AudioFileReader("./current-vs-last-release/test.mp3");

        [Benchmark(Baseline = true)]
        public List<DtmfOccurence> LastRelease() => audioFile.DtmfTones().ToList();

        [Benchmark]
        public List<DtmfChange> CurrentRelease() => audioFile.DtmfChanges();
    }
}

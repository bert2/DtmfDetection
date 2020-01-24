namespace DtmfDetection.NAudio {
    using System.Collections.Generic;
    using global::NAudio.Wave;

    public static class WaveStreamExtensions {
        public static List<DtmfChange> DtmfChanges(this WaveStream waveStream, bool forceMono = true) {
            var a = Analyzer.Create(new AudioFile(waveStream, Config.Default.SampleRate, forceMono), Config.Default);
            var ds = new List<DtmfChange>();
            while (a.MoreSamplesAvailable) ds.AddRange(a.AnalyzeNextBlock());
            return ds;
        }
    }
}

namespace DtmfDetection.NAudio {
    using System.Collections.Generic;
    using global::NAudio.Wave;

    public static class WaveStreamExt {
        public static List<DtmfChange> DtmfChanges(this WaveStream waveStream, bool forceMono = true, in Config? config = null) {
            var cfg = config ?? Config.Default;
            var analyzer = Analyzer.Create(
                new AudioFile(waveStream, cfg.SampleRate, forceMono),
                cfg);

            var dtmfs = new List<DtmfChange>();

            while (analyzer.MoreSamplesAvailable)
                dtmfs.AddRange(analyzer.AnalyzeNextBlock());

            return dtmfs;
        }
    }
}

namespace DtmfDetection.NAudio {
    using System.Collections.Generic;
    using global::NAudio.Wave;

    /// <summary>Provides an extension method to detect DTMF tones in a `WaveStream`.</summary>
    public static class WaveStreamExt {
        /// <summary>Detects DTMF tones in a `WaveStream`.</summary>
        /// <param name="waveStream">The input audio data as a `WaveStream`.</param>
        /// <param name="forceMono">Toggles conversion of multi-channel audio to mono before the analysis.</param>
        /// <param name="config">Optional detector configuration. Defaults to `Config.Default`.</param>
        /// <returns>All detected DTMF tones as a list of `DtmfChange`s.</returns>
        public static List<DtmfChange> DtmfChanges(this WaveStream waveStream, bool forceMono = true, Config? config = null) {
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

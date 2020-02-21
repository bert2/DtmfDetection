namespace DtmfDetection {
    using System.Collections.Generic;

    public static class FloatArrayExt {
        public static List<DtmfChange> DtmfChanges(
            this float[] samples,
            int channels = 1,
            int sampleRate = Config.DefaultSampleRate,
            in Config? config = null) {

            var cfg = config ?? Config.Default;
            var audio = new AudioData(samples, channels, sampleRate);
            var analyzer = Analyzer.Create(audio, cfg);

            var dtmfs = new List<DtmfChange>();

            while (analyzer.MoreSamplesAvailable)
                dtmfs.AddRange(analyzer.AnalyzeNextBlock());

            return dtmfs;
        }
    }
}

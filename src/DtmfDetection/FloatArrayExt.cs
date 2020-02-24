namespace DtmfDetection {
    using System.Collections.Generic;

    /// <summary>Provides an extension method to detect DTMF tones in PCM audio data.</summary>
    public static class FloatArrayExt {
        /// <summary>Detects DTMF tones in an array of `float`s.</summary>
        /// <param name="samples">The input audio data as an array of `float` values.</param>
        /// <param name="channels">The number of audio channels in the input data.</param>
        /// <param name="sampleRate">The sample rate (in Hz) at which the input data was sampled.</param>
        /// <param name="config">Optional detector configuration. Defaults to `Config.Default`.</param>
        /// <returns>All detected DTMF tones as a list of `DtmfChange`s.</returns>
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

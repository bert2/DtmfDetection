namespace DtmfDetection.NAudio {
    using System;
    using System.Threading;
    using DtmfDetection.Interfaces;
    using global::NAudio.Wave;

    /// <summary>Helper that does audio analysis in a background thread. Useful when analyzing infinite inputs like mic-in our the current audio output.</summary>
    public class BackgroundAnalyzer : IDisposable {
        private readonly IWaveIn source;
        private readonly AudioStream samples;
        private readonly IAnalyzer analyzer;
        private Thread? captureWorker;

        /// <summary>Fired when a DTMF change (a DTMF tone started or stopped) has been detected.</summary>
        public event Action<DtmfChange>? OnDtmfDetected;

        /// <summary>Creates a new `BackgroundAnalyzer` and immediately starts listening to the `IWaveIn` input. `Dispose()` this instance to stop the background thread doing the analysis.</summary>
        /// <param name="source">The input data. Must not be in recording state.</param>
        /// <param name="forceMono">Toggles conversion of multi-channel audio to mono before the analysis.</param>
        /// <param name="onDtmfDetected">Optional handler for the `OnDtmfDetected` event.</param>
        /// <param name="config">Optional detector configuration. Defaults to `Config.Default`.</param>
        /// <param name="analyzer">Optional; can be used to inject a custom analyzer implementation. Defaults to `Analyzer`.</param>
        public BackgroundAnalyzer(
            IWaveIn source,
            bool forceMono = true,
            Action<DtmfChange>? onDtmfDetected = null,
            Config? config = null,
            IAnalyzer? analyzer = null) {
            this.source = source;
            OnDtmfDetected += onDtmfDetected;
            var cfg = config ?? Config.Default;
            samples = new AudioStream(source, cfg.SampleRate, forceMono);
            this.analyzer = analyzer ?? Analyzer.Create(samples, cfg);
            StartCapturing();
        }

        /// <summary>Calls `IWaveIn.StopRecording()` on the input stream and halts the background thread doing the analysis.</summary>
        public void Dispose() => StopCapturing();

        private void StartCapturing() {
            source.StartRecording();
            captureWorker = new Thread(Analyze);
            captureWorker.Start();
        }

        private void StopCapturing() {
            source.StopRecording();
            samples.StopWaiting();
            captureWorker?.Join();
        }

        private void Analyze() {
            while (analyzer.MoreSamplesAvailable) {
                foreach (var dtmf in analyzer.AnalyzeNextBlock())
                    OnDtmfDetected?.Invoke(dtmf);
            }
        }
    }
}

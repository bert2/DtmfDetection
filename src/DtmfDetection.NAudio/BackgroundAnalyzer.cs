namespace DtmfDetection.NAudio {
    using System;
    using System.Threading;
    using DtmfDetection.Interfaces;
    using global::NAudio.Wave;

    public class BackgroundAnalyzer : IDisposable {
        private readonly IWaveIn source;
        private readonly AudioStream samples;
        private readonly IAnalyzer analyzer;
        private Thread? captureWorker;

        public event Action<DtmfChange>? OnDtmfDetected;

        public BackgroundAnalyzer(IWaveIn source, bool forceMono = true, in Config? config = null, IAnalyzer? analyzer = null) {
            this.source = source;
            var cfg = config ?? Config.Default;
            samples = new AudioStream(source, cfg.SampleRate, forceMono);
            this.analyzer = analyzer ?? Analyzer.Create(samples, cfg);
            StartCapturing();
        }

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

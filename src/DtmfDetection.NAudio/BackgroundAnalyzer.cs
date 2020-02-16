namespace DtmfDetection.NAudio {
    using System;
    using System.Threading;
    using DtmfDetection.Interfaces;
    using global::NAudio.Wave;

    public class BackgroundAnalyzer {
        private readonly IWaveIn source;
        private readonly IAnalyzer analyzer;
        private Thread? captureWorker;
        private volatile bool stopRequested = false;

        public event Action<DtmfChange>? OnDtmfDetected;

        public bool IsCapturing { get; private set; }

        public BackgroundAnalyzer(IWaveIn source, bool forceMono = true, in Config? config = null, IAnalyzer? analyzer = null) {
            this.source = source;
            var cfg = config ?? Config.Default;
            var samples = new AudioStream(source, cfg.SampleRate, forceMono);
            this.analyzer = analyzer ?? Analyzer.Create(samples, cfg);
        }

        public void StartCapturing() {
            if (IsCapturing) return;

            IsCapturing = true;
            source.StartRecording();
            captureWorker = new Thread(Analyze);
            captureWorker.Start();
        }

        public void StopCapturing() {
            if (!IsCapturing) return;

            IsCapturing = false;
            source.StopRecording();
            stopRequested = true;
            captureWorker?.Join();
        }

        private void Analyze() {
            while (!stopRequested && analyzer.MoreSamplesAvailable) {
                foreach (var dtmf in analyzer.AnalyzeNextBlock())
                    OnDtmfDetected?.Invoke(dtmf);
            }
        }
    }
}

namespace Integration {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using DtmfDetection;
    using DtmfDetection.NAudio;
    using NAudio.Wave;
    using Shouldly;
    using Xunit;

    public class BackgroundAnalyzerTests {
        [Fact]
        public void DetectsDtmfTonesInShortSequence() {
            var dtmfs = new List<DtmfChange>();
            using var waveIn = new FakeWaveIn("./testdata/short_dtmf_sequence.mp3");
            using var analyzer = new BackgroundAnalyzer(waveIn, onDtmfDetected: dtmf => dtmfs.Add(dtmf));
            Thread.Sleep(3000);
            dtmfs.Count.ShouldBe(8, string.Join("\n\t", dtmfs));
        }

        private class FakeWaveIn : IWaveIn {
            public WaveFormat WaveFormat { get => reader.WaveFormat; set => throw new InvalidOperationException(); }
            public event EventHandler<WaveInEventArgs>? DataAvailable;
            public event EventHandler<StoppedEventArgs>? RecordingStopped;
            private readonly AudioFileReader reader;
            private bool stopRequested;

            public FakeWaveIn(string audioFile) => reader = new AudioFileReader(audioFile);

            public void StartRecording() => ThreadPool.QueueUserWorkItem(_ => Record(), null);

            public void StopRecording() => stopRequested = true;

            public void Dispose() => reader.Dispose();

            private void Record() {
                try {
                    while (!stopRequested && reader.CanRead) {
                        const int n = 10000;
                        var buffer = new byte[n];
                        var bytes = reader.Read(buffer, 0, n);
                        if (bytes == 0) break;
                        DataAvailable?.Invoke(this, new WaveInEventArgs(buffer, bytes));
                        Thread.Sleep(1);
                    }
                } catch (Exception e) {
                    RecordingStopped?.Invoke(this, new StoppedEventArgs(e));
                }
                finally {
                    RecordingStopped?.Invoke(this, new StoppedEventArgs());
                }
            }
        }
    }
}

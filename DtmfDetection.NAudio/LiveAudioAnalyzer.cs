namespace DtmfDetection.NAudio
{
    using System;
    using System.Threading;

    using global::NAudio.Wave;

    public class LiveAudioAnalyzer
    {
        private readonly IWaveIn waveIn;

        private readonly DtmfAudio dtmfAudio;

        private Thread captureWorker;

        public event Action<DtmfToneStart> DtmfToneStarting;

        public event Action<DtmfToneEnd> DtmfToneStopped;

        public LiveAudioAnalyzer(IWaveIn waveIn)
        {
            this.waveIn = waveIn;
            dtmfAudio = new DtmfAudio(new StreamingSampleSource(Buffer(waveIn)));
        }

        public bool IsCapturing { get; private set; }

        public void StartCapturing()
        {
            if (IsCapturing)
                return;

            IsCapturing = true;
            waveIn.StartRecording();
            captureWorker = new Thread(Detect);
            captureWorker.Start();
        }

        public void StopCapturing()
        {
            if (!IsCapturing)
                return;

            IsCapturing = false;
            waveIn.StopRecording();
            captureWorker.Abort();
            captureWorker.Join();
        }

        private void Detect()
        {
            var next = DtmfTone.None;

            while (true)
            {
                var current = next != DtmfTone.None ? next : dtmfAudio.Wait();

                var start = DateTime.Now;
                DtmfToneStarting?.Invoke(new DtmfToneStart(current, start));

                next = dtmfAudio.Skip();

                var duration = DateTime.Now - start;
                DtmfToneStopped?.Invoke(new DtmfToneEnd(current, duration));
            }
        }

        private static BufferedWaveProvider Buffer(IWaveIn source)
        {
            var sourceBuffer = new BufferedWaveProvider(source.WaveFormat) { DiscardOnBufferOverflow = true };
            source.DataAvailable += (sender, e) => sourceBuffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
            return sourceBuffer;
        }
    }
}

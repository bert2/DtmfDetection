namespace DtmfDetection.NAudio
{
    using System;
    using System.Threading;

    using global::NAudio.CoreAudioApi;
    using global::NAudio.Wave;

    public class CurrentAudioOutput
    {
        private readonly WasapiLoopbackCapture audioOutput;

        private readonly DtmfAudio dtmfAudio;

        private Thread captureWorker;

        public event Action<DtmfToneStart> DtmfToneStarting;

        public event Action<DtmfToneEnd> DtmfToneStopped;

        public CurrentAudioOutput()
        {
            audioOutput = new WasapiLoopbackCapture { ShareMode = AudioClientShareMode.Shared };
            dtmfAudio = new DtmfAudio(new StreamingSampleSource(Buffer(audioOutput)));
        }

        public void StartCapturing()
        {
            audioOutput.StartRecording();
            captureWorker = new Thread(Detect);
            captureWorker.Start();
        }

        public void StopCapturing()
        {
            audioOutput.StopRecording();
            captureWorker.Abort();
            captureWorker.Join();
        }

        private void Detect()
        {
            while (true)
            {
                var dtmfTone = dtmfAudio.WaitForDtmfTone();

                var start = DateTime.Now;
                DtmfToneStarting?.Invoke(new DtmfToneStart(dtmfTone, start));

                dtmfAudio.WaitForEndOfLastDtmfTone();

                var duration = DateTime.Now - start;
                DtmfToneStopped?.Invoke(new DtmfToneEnd(dtmfTone, duration));
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

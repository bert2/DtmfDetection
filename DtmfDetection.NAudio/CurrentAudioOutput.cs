namespace DtmfDetection.NAudio
{
    using System;
    using System.Threading;

    using global::NAudio.CoreAudioApi;
    using global::NAudio.Wave;

    public class CurrentAudioOutput
    {
        private readonly WasapiLoopbackCapture audioOutput;

        private readonly StreamingDtmfAudio dtmfAudio;

        private Thread captureWorker;

        public event Action<DtmfToneStart> DtmfToneStarting;

        public event Action<DtmfToneEnd> DtmfToneStopped;

        public CurrentAudioOutput()
        {
            audioOutput = new WasapiLoopbackCapture { ShareMode = AudioClientShareMode.Shared };
            dtmfAudio = new StreamingDtmfAudio(Buffer(audioOutput));
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
                dtmfAudio.WaitForDtmfTone();

                var start = DateTime.Now;
                DtmfToneStarting?.Invoke(new DtmfToneStart(dtmfAudio.LastDtmfTone, start));

                dtmfAudio.WaitForEndOfDtmfTone();

                var duration = DateTime.Now - start;
                DtmfToneStopped?.Invoke(new DtmfToneEnd(dtmfAudio.LastDtmfTone, duration));
            }
        }

        private static BufferedWaveProvider Buffer(IWaveIn source)
        {
            var sourceBuffer = new BufferedWaveProvider(source.WaveFormat) { DiscardOnBufferOverflow = true };
            source.DataAvailable += (sender, e) =>
                                    {
                                        sourceBuffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
                                        Console.WriteLine($"Added {e.BytesRecorded}");
                                    };
            return sourceBuffer;
        }
    }
}

namespace DtmfDetection.NAudio
{
    using System;
    using System.Threading;

    using global::NAudio.Wave;

    /// <summary>Implements means to detect DTMF tones in audio data provided as NAudio's IWaveIn.
    /// The detection runs in a background thread and can be controlled with the methods
    /// Start/StopCapturing(). Hook up to the DtmfToneStarted/Stopped events to receive the beginnings
    /// and ends of DTMF tones.</summary>
    /// <remarks>By default a LiveAudioDtmfAnalyzer forces a mono conversion by averaging all audio 
    /// channels first. Turn it off with the constructor's forceMono flag in order to analyze each 
    /// channel separately.</remarks>
    public class LiveAudioDtmfAnalyzer
    {
        private readonly IWaveIn waveIn;

        private readonly DtmfAudio dtmfAudio;

        private Thread captureWorker;

        /// <summary>Raised when a DTMF tone has been detected.</summary>
        public event Action<DtmfToneStart> DtmfToneStarted;

        /// <summary>Raised when the end of a DTMF tone has been detected.</summary>
        public event Action<DtmfToneEnd> DtmfToneStopped;

        /// <summary>Creates a new instance of LiveAudioDtmfAnalyzer.</summary>
        /// <param name="waveIn">The audio data source.</param>
        /// <param name="forceMono">Indicates whether the audio data should be converted to mono 
        /// first. Default is true.</param>
        public LiveAudioDtmfAnalyzer(IWaveIn waveIn, bool forceMono = true)
        {
            this.waveIn = waveIn;
            var config = new DetectorConfig();
            dtmfAudio = DtmfAudio.CreateFrom(new StreamingSampleSource(config, Buffer(waveIn), forceMono), config);
        }

        /// <summary>Indicates whether the background thread is running or not.</summary>
        public bool IsCapturing { get; private set; }

        /// <summary>Starts analyzing the audio data in a background thread. Does nothing if the
        /// analyzer is already running.</summary>
        public void StartCapturing()
        {
            if (IsCapturing)
                return;

            IsCapturing = true;
            waveIn.StartRecording();
            captureWorker = new Thread(Detect);
            captureWorker.Start();
        }

        /// <summary>Stops analyzing the audio data and the background thread. Does nothing if the
        /// analyzer is not running.</summary>
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
            while (dtmfAudio.Forward(
                (channel, tone) =>
                {
                    var start = DateTime.Now;
                    DtmfToneStarted?.Invoke(new DtmfToneStart(tone, channel, start));
                    return start;
                },
                (channel, start, tone) => DtmfToneStopped?.Invoke(new DtmfToneEnd(tone, channel, DateTime.Now - start))))
            {
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

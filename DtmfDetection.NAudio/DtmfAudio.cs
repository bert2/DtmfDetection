namespace DtmfDetection.NAudio
{
    using global::NAudio.Wave;

    public class DtmfAudio
    {
        private readonly DtmfDetector dtmfDetector = new DtmfDetector();

        private readonly SampleBlockProvider samples;

        public DtmfAudio(IWaveProvider audio)
        {
            samples = audio.AsMono()
                           .SampleWith(DtmfDetector.SampleRate)
                           .Blockwise(DtmfDetector.SampleBlockSize);
        }

        public DtmfTone LastDtmfTone { get; private set; } = DtmfTone.None;

        public bool SkipToDtmfTone()
        {
            while (samples.ReadNextBlock())
            {
                LastDtmfTone = dtmfDetector.FindDtmfTone(samples.CurrentBlock);

                if (LastDtmfTone != DtmfTone.None)
                    return true;
            }

            return false;
        }

        public void SkipToEndOfDtmfTone()
        {
            while (samples.ReadNextBlock())
            {
                var nextDtmfTone = dtmfDetector.FindDtmfTone(samples.CurrentBlock);

                if (nextDtmfTone == LastDtmfTone)
                    continue;

                return;
            }
        }
    }
}
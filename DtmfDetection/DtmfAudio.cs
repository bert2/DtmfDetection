namespace DtmfDetection
{
    public class DtmfAudio
    {
        private readonly ISampleSource source;

        private readonly DtmfDetector dtmfDetector = new DtmfDetector();

        public DtmfAudio(ISampleSource source)
        {
            this.source = source;
        }

        public DtmfTone LastDtmfTone { get; private set; } = DtmfTone.None;

        public DtmfTone WaitForDtmfTone()
        {
            while (source.HasSamples)
            {
                LastDtmfTone = dtmfDetector.Analyze(source.Samples);

                if (LastDtmfTone != DtmfTone.None)
                    return LastDtmfTone;
            }

            return DtmfTone.None;
        }

        public void WaitForEndOfLastDtmfTone()
        {
            while (source.HasSamples)
            {
                var nextDtmfTone = dtmfDetector.Analyze(source.Samples);

                if (nextDtmfTone != LastDtmfTone)
                    return;
            }
        }
    }
}
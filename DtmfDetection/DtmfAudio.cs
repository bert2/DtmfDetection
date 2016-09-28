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

        public DtmfTone CurrentDtmfTone { get; private set; } = DtmfTone.None;

        public DtmfTone Wait()
        {
            while (source.HasSamples)
            {
                CurrentDtmfTone = dtmfDetector.Analyze(source.Samples);

                if (CurrentDtmfTone != DtmfTone.None)
                    return CurrentDtmfTone;
            }

            return DtmfTone.None;
        }

        public DtmfTone Skip()
        {
            while (source.HasSamples)
            {
                var nextDtmfTone = dtmfDetector.Analyze(source.Samples);

                if (nextDtmfTone == CurrentDtmfTone)
                    continue;

                CurrentDtmfTone = nextDtmfTone;
                return nextDtmfTone;
            }

            return DtmfTone.None;
        }
    }
}
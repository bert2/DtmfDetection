namespace DtmfDetection
{
    public class DtmfAudio
    {
        private readonly ISampleSource source;

        private readonly DtmfDetector dtmfDetector;

        public DtmfAudio(DtmfDetector dtmfDetector, ISampleSource source)
        {
            this.source = source;
            this.dtmfDetector = dtmfDetector;
        }

        public static DtmfAudio CreateFrom(ISampleSource source, DetectorConfig config)
        {
            return new DtmfAudio(
                new DtmfDetector(config, new PureTones(new AmplitudeEstimatorFactory(source.SampleRate, config.SampleBlockSize))),
                source);
        }

        public DtmfTone CurrentDtmfTone { get; private set; } = DtmfTone.None;

        public DtmfTone Wait()
        {
            if (CurrentDtmfTone != DtmfTone.None)
                return CurrentDtmfTone;

            while (source.HasSamples)
            {
                CurrentDtmfTone = dtmfDetector.Analyze(source.Samples);

                if (CurrentDtmfTone != DtmfTone.None)
                    return CurrentDtmfTone;
            }

            return DtmfTone.None;
        }

        public void Skip()
        {
            while (source.HasSamples)
            {
                var nextDtmfTone = dtmfDetector.Analyze(source.Samples);

                if (nextDtmfTone != CurrentDtmfTone)
                {
                    CurrentDtmfTone = nextDtmfTone;
                    return;
                }
            }

            CurrentDtmfTone = DtmfTone.None;
        }
    }
}
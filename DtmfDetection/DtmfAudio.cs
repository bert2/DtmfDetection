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

        public DtmfOccurence CurrentDtmfTone { get; private set; } = new DtmfOccurence(DtmfTone.None, -1);

        public DtmfOccurence Wait()
        {
            while (source.HasSamples)
            {
                CurrentDtmfTone = new DtmfOccurence(dtmfDetector.Analyze(source.Samples), 0);

                if (CurrentDtmfTone.Tone != DtmfTone.None)
                    return CurrentDtmfTone;
            }

            return new DtmfOccurence(DtmfTone.None, -1);
        }

        public DtmfOccurence Skip()
        {
            while (source.HasSamples)
            {
                var nextDtmfTone = new DtmfOccurence(dtmfDetector.Analyze(source.Samples), 0);

                if (nextDtmfTone == CurrentDtmfTone)
                    continue;

                CurrentDtmfTone = nextDtmfTone;
                return nextDtmfTone;
            }

            return new DtmfOccurence(DtmfTone.None, -1);
        }
    }
}
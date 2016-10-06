namespace DtmfDetection
{
    using System;

    public class DtmfAudio
    {
        private readonly DtmfChangeHandler dtmfChangeHandler = new DtmfChangeHandler();

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
                new DtmfDetector(
                    config, 
                    new PureTones(
                        new AmplitudeEstimatorFactory(
                            source.SampleRate, 
                            config.SampleBlockSize))),
                source);
        }

        public bool Forward<TState>(Func<DtmfTone, TState> dtmfStarting, Action<TState, DtmfTone> dtmfStopping)
        {
            if (source.HasSamples)
                dtmfChangeHandler.Handle(dtmfDetector.Analyze(source.Samples), dtmfStarting, dtmfStopping);

            return source.HasSamples;
        }
    }
}
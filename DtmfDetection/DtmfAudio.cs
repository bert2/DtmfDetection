namespace DtmfDetection
{
    using System;
    using System.Linq;

    public class DtmfAudio
    {
        private readonly DtmfChangeHandler[] dtmfChangeHandlers;

        private readonly ISampleSource source;

        private readonly DtmfDetector dtmfDetector;

        private readonly int numChannels;

        public DtmfAudio(DtmfDetector dtmfDetector, ISampleSource source, DtmfChangeHandler[] dtmfChangeHandlers)
        {
            this.dtmfDetector = dtmfDetector;
            this.source = source;
            this.dtmfChangeHandlers = dtmfChangeHandlers;
            numChannels = source.Channels;
        }

        public static DtmfAudio CreateFrom(ISampleSource source, DetectorConfig config)
        {
            var pureTones = Enumerable
                .Range(0, source.Channels)
                .Select(c => new PureTones(new AmplitudeEstimatorFactory(source.SampleRate, config.SampleBlockSize)))
                .ToArray();

            var dtmfChangeHandlers = Enumerable
                .Range(0, source.Channels)
                .Select(c => new DtmfChangeHandler())
                .ToArray();

            return new DtmfAudio(new DtmfDetector(config, pureTones), source, dtmfChangeHandlers);
        }

        public bool Forward<TState>(Func<int, DtmfTone, TState> dtmfStarting, Action<int, TState, DtmfTone> dtmfStopping)
        {
            // Save value of HasSamples, because it might be different after analyzing (i.e. reading).
            var canAnalyze = source.HasSamples;

            var dtmfTones = canAnalyze
                ? dtmfDetector.Analyze(source.Samples)
                // Reached end of data: generate DtmfTone.None's and flush the state machines to handle cut-off tones.
                : Enumerable.Repeat(DtmfTone.None, numChannels).ToArray();

            for (var channel = 0; channel < numChannels; channel++)
            {
                dtmfChangeHandlers[channel].Handle(
                    dtmfTones[channel],
                    dtmfStarting.Apply(channel),
                    dtmfStopping.Apply(channel));
            }

            return canAnalyze;
        }
    }
}
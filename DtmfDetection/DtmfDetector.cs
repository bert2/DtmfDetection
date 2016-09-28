namespace DtmfDetection
{
    using System.Collections.Generic;
    using System.Linq;

    public class DtmfDetector
    {
        public const int SampleRate = 8000;

        // We assume DTMF tones with a length of at least 40 ms: 8000 Hz * 0.04 s = 320 (sample blocks).
        public const int SampleBlockSize = SampleRate * 40 / 1000;

        private const double AmplitudeThreshold = 10.0;

        private readonly PureTones pureTones = new PureTones(SampleRate, SampleBlockSize);

        public DtmfTone Analyze(IEnumerable<float> samples)
        {
            pureTones.ResetAmplitudes();

            foreach (var sample in samples.Take(SampleBlockSize))
                pureTones.AddSample(sample);

            return GetDtmfToneFromAmplitudes();
        }

        private DtmfTone GetDtmfToneFromAmplitudes()
        {
            var highTone = pureTones.FindStrongestHighTone();
            var lowTone = pureTones.FindStrongestLowTone();

            if (pureTones[highTone] < AmplitudeThreshold || pureTones[lowTone] < AmplitudeThreshold)
                return DtmfTone.None;

            return DtmfClassification.For(highTone, lowTone);
        }
    }
}
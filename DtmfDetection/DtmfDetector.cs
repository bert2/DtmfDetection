using System;

namespace DtmfDetection
{
    using System.Collections.Generic;
    using System.Linq;

    public class DtmfDetector
    {
        private const double AmplitudeThreshold = 10.0;

        private readonly PureTones pureTones = new PureTones(SampleRate, SampleBlockSize);

        public DtmfTone Analyze(IEnumerable<float> samples)
        {
            pureTones.ResetAmplitudes();

            foreach (var sample in samples.Take(SampleBlockSize))
                pureTones.AddSample(sample);

            return GetDtmfToneFromAmplitudes();
        }

        public static int SampleRate { get; } = 8000;

        // A DTMF Tone has to have a length of at least 40 ms: 8000 Hz * 0.04 s = 320
        public static int SampleBlockSize { get; } = SampleRate * 40 / 1000;

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
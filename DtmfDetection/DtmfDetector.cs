namespace DtmfDetection
{
    using System.Collections.Generic;
    using System.Linq;

    public class DtmfDetector
    {
        public const int SampleRate = 8000;

        // Using 205 samples minimizes error (distance of DTMF frequency to center of DFT bin).
        public const int SampleBlockSize = 205;

        private const double Threshold = 100.0;

        private readonly PureTones powers = new PureTones(SampleRate, SampleBlockSize);

        public DtmfTone Analyze(IEnumerable<float> samples)
        {
            powers.ResetAmplitudes();

            foreach (var sample in samples.Take(SampleBlockSize))
                powers.AddSample(sample);

            return GetDtmfToneFromPowers();
        }

        private DtmfTone GetDtmfToneFromPowers()
        {
            var highTone = powers.FindStrongestHighTone();
            var lowTone = powers.FindStrongestLowTone();

            if (powers[highTone] < Threshold || powers[lowTone] < Threshold)
                return DtmfTone.None;

            return DtmfClassification.For(highTone, lowTone);
        }
    }
}
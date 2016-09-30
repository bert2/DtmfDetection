namespace DtmfDetection
{
    using System.Collections.Generic;
    using System.Linq;

    public class DtmfDetector
    {
        private readonly DetectorConfig config;

        private readonly PureTones powers;

        public DtmfDetector(DetectorConfig config, PureTones powers)
        {
            this.config = config;
            this.powers = powers;
        }

        public DtmfTone Analyze(IEnumerable<float> samples)
        {
            powers.ResetAmplitudes();

            foreach (var sample in samples.Take(config.SampleBlockSize))
                powers.AddSample(sample);

            return GetDtmfToneFromPowers();
        }

        private DtmfTone GetDtmfToneFromPowers()
        {
            var highTone = powers.FindStrongestHighTone();
            var lowTone = powers.FindStrongestLowTone();

            if (powers[highTone] < config.PowerThreshold || powers[lowTone] < config.PowerThreshold)
                return DtmfTone.None;

            return DtmfClassification.For(highTone, lowTone);
        }
    }
}
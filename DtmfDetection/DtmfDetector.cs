namespace DtmfDetection
{
    using System.Collections.Generic;
    using System.Linq;

    public class DtmfDetector
    {
        private readonly DetectorConfig config;

        private readonly PureTones[] powers;

        private readonly int numChannels;

        public DtmfDetector(DetectorConfig config, PureTones[] powers)
        {
            this.config = config;
            this.powers = powers;
            numChannels = powers.Length;
        }

        public DtmfTone[] Analyze(IEnumerable<float> samples)
        {
            foreach (var p in powers)
                p.ResetAmplitudes();

            var channel = 0;
            foreach (var sample in samples.Take(config.SampleBlockSize * numChannels))
            {
                powers[channel].AddSample(sample);
                channel = (channel + 1) % numChannels;
            }

            return powers
                .Select(p => GetDtmfToneFromPowers(p, config.PowerThreshold))
                .ToArray();
        }

        private static DtmfTone GetDtmfToneFromPowers(PureTones powers, double threshold)
        {
            var highTone = powers.FindStrongestHighTone();
            var lowTone = powers.FindStrongestLowTone();

            if (powers[highTone] < threshold || powers[lowTone] < threshold)
                return DtmfTone.None;

            return DtmfClassification.For(highTone, lowTone);
        }
    }
}
namespace DtmfDetection
{
    using System.Collections.Generic;
    using System.Linq;

    public class PureTones
    {
        private static readonly IEnumerable<int> LowPureTones = new[] { 697, 770, 852, 941 };

        private static readonly IEnumerable<int> HighPureTones = new[] { 1209, 1336, 1477, 1633 };

        private readonly Dictionary<int, AmplitudeEstimator> estimators;

        public PureTones(AmplitudeEstimatorFactory estimatorFactory)
        {
            estimators = LowPureTones
                .Concat(HighPureTones)
                .ToDictionary(tone => tone, estimatorFactory.CreateFor);
        }

        public double this[int tone] => estimators[tone].AmplitudeSquared;

        public void ResetAmplitudes()
        {
            foreach (var estimator in estimators.Values)
                estimator.Reset();
        }

        public void AddSample(float sample)
        {
            foreach (var estimator in estimators.Values)
                estimator.Add(sample);
        }

        public int FindStrongestHighTone() => StrongestOf(HighPureTones);

        public int FindStrongestLowTone() => StrongestOf(LowPureTones);

        private int StrongestOf(IEnumerable<int> pureTones)
        {
            return pureTones.Select(tone => new
                                            {
                                                Tone = tone,
                                                Power = estimators[tone].AmplitudeSquared
                                            })
                            .OrderBy(result => result.Power)
                            .Select(result => result.Tone)
                            .Last();
        }
    }
}
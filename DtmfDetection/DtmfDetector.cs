namespace DtmfDetection
{
    using System.Collections.Generic;
    using System.Linq;

    public class DtmfDetector
    {
        public const int SampleRate = 8000;

        // A DTMF tone has to have a length of at least 40 ms: 8000 Hz * 0.04 ms = 320
        public const int SampleBlockSize = SampleRate * 40 / 1000;

        private const double AmplitudeThreshold = 10.0;

        private static IEnumerable<int> HighTones { get; } = new[] { 1209, 1336, 1477, 1633 };

        private static IEnumerable<int> LowTones { get; } = new[] { 697, 770, 852, 941 };

        private readonly Dictionary<int, FourierTransform> estimate = LowTones.Concat(HighTones)
                                                                              .ToDictionary(tone => tone,
                                                                                            tone => new FourierTransform(tone, SampleRate, SampleBlockSize));

        public DtmfTone FindDtmfTone(float[] sampleBlock)
        {
            var lowTone = StrongestOf(LowTones, sampleBlock);

            if (!lowTone.HasValue)
                return DtmfTone.None;

            var highTone = StrongestOf(HighTones, sampleBlock);

            if (!highTone.HasValue)
                return DtmfTone.None;

            return DtmfClassification.For(highTone.Value, lowTone.Value);
        }

        private int? StrongestOf(IEnumerable<int> dtmfTones, float[] sampleBlock)
        {
            return dtmfTones.Select(tone => new
                                            {
                                                Frequency = tone,
                                                Amplitude = estimate[tone].AmplitudeIn(sampleBlock)
                                            })
                            .Where(result => result.Amplitude > AmplitudeThreshold)
                            .OrderBy(result => result.Amplitude)
                            .Select(result => (int?)result.Frequency)
                            .LastOrDefault();
        }
    }
}
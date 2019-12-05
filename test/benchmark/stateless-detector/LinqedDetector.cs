namespace Benchmark.StatelessDetector {
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;

    public class LinqedDetector {
        private static readonly int[] lowTones = new[] { 697, 770, 852, 941 };
        private static readonly int[] highTones = new[] { 1209, 1336, 1477, 1633 };
        private readonly int sampleRate;
        private readonly int numSamples;
        private readonly double threshold;
        private readonly IEnumerable<Goertzel> lows;
        private readonly IEnumerable<Goertzel> highs;

        public LinqedDetector() {
            sampleRate = 8000;
            numSamples = 205;
            threshold = 35.0;
            lows = lowTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
            highs = highTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
        }

        public PhoneKey Analyze(IEnumerable<float> sampleBlock) {
            var (loResps, hiResps) = sampleBlock
                .Take(numSamples)
                .Aggregate(
                    (lows, highs),
                    (goertzel, sample) => (goertzel.lows.Select(g => g.AddSample(sample)), goertzel.highs.Select(g => g.AddSample(sample))));

            var (fstLowIdx, fstLow, sndLowIdx, sndLow) = FindMaxTwo(loResps);
            var (fstHighIdx, fstHigh, sndHighIdx, sndHigh) = FindMaxTwo(hiResps);

            return fstLow < threshold || fstHigh < threshold
                || fstLow > threshold && sndLow > threshold
                || fstHigh > threshold && sndHigh > threshold
                ? PhoneKey.None
                : (highTones[fstHighIdx], lowTones[fstLowIdx]).ToPhoneKey();
        }

        private static (int fstIdx, double fstVal, int sndIdx, double sndVal) FindMaxTwo(IEnumerable<Goertzel> goertzels) {
            int fstIdx = 0, sndIdx = 1;
            double fstVal = goertzels.First().NormResponse, sndVal = goertzels.Skip(1).First().NormResponse;

            foreach (var (g, i) in goertzels.Skip(1).Select((g, i) => (g, i))) {
                if (g.NormResponse > fstVal) {
                    sndIdx = fstIdx;
                    sndVal = fstVal;
                    fstIdx = i;
                    fstVal = g.NormResponse;
                } else if (g.NormResponse > sndVal) {
                    sndIdx = i;
                    sndVal = g.NormResponse;
                }
            }

            return (fstIdx, fstVal, sndIdx, sndVal);
        }
    }
}

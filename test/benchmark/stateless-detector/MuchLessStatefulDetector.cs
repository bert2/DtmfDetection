namespace Benchmark.StatelessDetector {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;

    public class MuchLessStatefulDetector {
        private static readonly int[] lowTones = new[] { 697, 770, 852, 941 };
        private static readonly int[] highTones = new[] { 1209, 1336, 1477, 1633 };
        private readonly int sampleRate;
        private readonly int numSamples;
        private readonly double threshold;
        private readonly Goertzel[] lows;
        private readonly Goertzel[] highs;

        public MuchLessStatefulDetector() {
            sampleRate = 8000;
            numSamples = 205;
            threshold = 35.0;
            lows = lowTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
            highs = highTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
        }

        public PhoneKey Analyze(IEnumerable<float> sampleBlock) {
            var (loResps, hiResps) = sampleBlock.Take(numSamples).Aggregate((lows, highs), (goertzel, sample) =>
                (new[] {
                    goertzel.lows[0].AddSample(sample),
                    goertzel.lows[1].AddSample(sample),
                    goertzel.lows[2].AddSample(sample),
                    goertzel.lows[3].AddSample(sample)
                },
                new[] {
                    goertzel.highs[0].AddSample(sample),
                    goertzel.highs[1].AddSample(sample),
                    goertzel.highs[2].AddSample(sample),
                    goertzel.highs[3].AddSample(sample)
                }));

            var (fstLowIdx, sndLowIdx) = FindMaxTwo(loResps);
            var (fstLow, sndLow) = (loResps[fstLowIdx].NormResponse, loResps[sndLowIdx].NormResponse);

            var (fstHighIdx, sndHighIdx) = FindMaxTwo(hiResps);
            var (fstHigh, sndHigh) = (hiResps[fstHighIdx].NormResponse, hiResps[sndHighIdx].NormResponse);

            return fstLow < threshold || fstHigh < threshold
                || fstLow > threshold && sndLow > threshold
                || fstHigh > threshold && sndHigh > threshold
                ? PhoneKey.None
                : (highTones[fstHighIdx], lowTones[fstLowIdx]).ToPhoneKey();
        }

        private static (int fstIdx, int sndIdx) FindMaxTwo(in ReadOnlySpan<Goertzel> goertzels) {
            int fst = 0, snd = 1;

            for (var i = 1; i < 4; i++) {
                if (goertzels[i].NormResponse > goertzels[fst].NormResponse) {
                    snd = fst;
                    fst = i;
                } else if (goertzels[i].NormResponse > goertzels[snd].NormResponse) {
                    snd = i;
                }
            }

            return (fst, snd);
        }
    }
}

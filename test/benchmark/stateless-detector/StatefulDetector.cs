namespace Benchmark.StatelessDetector {
    using System;
    using System.Linq;
    using DtmfDetection;

    public class StatefulDetector {
        private static readonly int[] lowTones = new[] { 697, 770, 852, 941 };
        private static readonly int[] highTones = new[] { 1209, 1336, 1477, 1633 };
        private readonly int sampleRate;
        private readonly int numSamples;
        private readonly double threshold;
        private readonly Goertzel[] lows;
        private readonly Goertzel[] highs;

        public StatefulDetector() {
            sampleRate = 8000;
            numSamples = 205;
            threshold = 35.0;
            lows = lowTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
            highs = highTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
        }

        public PhoneKey Analyze(in ReadOnlySpan<float> sampleBlock) {
            ResetGoertzelResponses();
            var length = Math.Min(numSamples, sampleBlock.Length);

            for (var i = 0; i < length; i++) {
                lows[0] = lows[0].AddSample(sampleBlock[i]);
                lows[1] = lows[1].AddSample(sampleBlock[i]);
                lows[2] = lows[2].AddSample(sampleBlock[i]);
                lows[3] = lows[3].AddSample(sampleBlock[i]);

                highs[0] = highs[0].AddSample(sampleBlock[i]);
                highs[1] = highs[1].AddSample(sampleBlock[i]);
                highs[2] = highs[2].AddSample(sampleBlock[i]);
                highs[3] = highs[3].AddSample(sampleBlock[i]);
            }

            var (fstLowIdx, sndLowIdx) = FindMaxTwo(lows);
            var (fstLow, sndLow) = (lows[fstLowIdx].NormResponse, lows[sndLowIdx].NormResponse);

            var (fstHighIdx, sndHighIdx) = FindMaxTwo(highs);
            var (fstHigh, sndHigh) = (highs[fstHighIdx].NormResponse, highs[sndHighIdx].NormResponse);

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

        private void ResetGoertzelResponses() {
            lows[0] = lows[0].Reset();
            lows[1] = lows[1].Reset();
            lows[2] = lows[2].Reset();
            lows[3] = lows[3].Reset();

            highs[0] = highs[0].Reset();
            highs[1] = highs[1].Reset();
            highs[2] = highs[2].Reset();
            highs[3] = highs[3].Reset();
        }
    }
}

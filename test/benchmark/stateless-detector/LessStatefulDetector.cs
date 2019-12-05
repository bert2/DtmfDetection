namespace Benchmark.StatelessDetector {
    using System;
    using System.Linq;
    using DtmfDetection;

    public class LessStatefulDetector {
        private static readonly int[] lowTones = new[] { 697, 770, 852, 941 };
        private static readonly int[] highTones = new[] { 1209, 1336, 1477, 1633 };
        private readonly int sampleRate;
        private readonly int numSamples;
        private readonly double threshold;
        private readonly Goertzel[] initLoResps;
        private readonly Goertzel[] initHiResps;

        public LessStatefulDetector() {
            sampleRate = 8000;
            numSamples = 205;
            threshold = 35.0;
            initLoResps = lowTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
            initHiResps = highTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
        }

        public PhoneKey Analyze(in ReadOnlySpan<float> sampleBlock) {
            var loResps = new[] { initLoResps[0], initLoResps[1], initLoResps[2], initLoResps[3] };
            var hiResps = new[] { initHiResps[0], initHiResps[1], initHiResps[2], initHiResps[3] };
            var length = Math.Min(numSamples, sampleBlock.Length);

            for (var i = 0; i < length; i++) {
                loResps[0] = loResps[0].AddSample(sampleBlock[i]);
                loResps[1] = loResps[1].AddSample(sampleBlock[i]);
                loResps[2] = loResps[2].AddSample(sampleBlock[i]);
                loResps[3] = loResps[3].AddSample(sampleBlock[i]);

                hiResps[0] = hiResps[0].AddSample(sampleBlock[i]);
                hiResps[1] = hiResps[1].AddSample(sampleBlock[i]);
                hiResps[2] = hiResps[2].AddSample(sampleBlock[i]);
                hiResps[3] = hiResps[3].AddSample(sampleBlock[i]);
            }

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

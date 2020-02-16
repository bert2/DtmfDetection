namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection.Interfaces;

    public class Detector : IDetector {
        private static readonly IReadOnlyList<int> lowTones = new[] { 697, 770, 852, 941 };
        private static readonly IReadOnlyList<int> highTones = new[] { 1209, 1336, 1477, 1633 };
        private readonly IReadOnlyList<Goertzel> initLoGoertz;
        private readonly IReadOnlyList<Goertzel> initHiGoertz;

        public int Channels { get; }

        public Config Config { get; }

        public Detector(int numChannels, in Config config) {
            Channels = numChannels;
            Config = config;

            var sampleRate = config.SampleRate;
            var numSamples = config.SampleBlockSize;
            initLoGoertz = lowTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
            initHiGoertz = highTones.Select(f => Goertzel.Init(f, sampleRate, numSamples)).ToArray();
        }

        public IReadOnlyList<PhoneKey> Detect(in ReadOnlySpan<float> sampleBlock) {
            var loGoertz = CreateGoertzels(initLoGoertz, Channels);
            var hiGoertz = CreateGoertzels(initHiGoertz, Channels);
            AddSamples(sampleBlock, Channels, loGoertz, hiGoertz);
            return Detect(loGoertz, hiGoertz, Config.Threshold, Channels);
        }

        private static Goertzel[][] CreateGoertzels(IReadOnlyList<Goertzel> initGoertz, int numChannels) {
            var goertz = new Goertzel[numChannels][];

            for (var c = 0; c < numChannels; c++) {
                goertz[c] = new[] { initGoertz[0], initGoertz[1], initGoertz[2], initGoertz[3] };
            }

            return goertz;
        }

        private static void AddSamples(in ReadOnlySpan<float> sampleBlock, int numChannels, Goertzel[][] loGoertz, Goertzel[][] hiGoertz) {
            for (var i = 0; i < sampleBlock.Length; i++) {
                var c = i % numChannels;

                loGoertz[c][0] = loGoertz[c][0].AddSample(sampleBlock[i]);
                loGoertz[c][1] = loGoertz[c][1].AddSample(sampleBlock[i]);
                loGoertz[c][2] = loGoertz[c][2].AddSample(sampleBlock[i]);
                loGoertz[c][3] = loGoertz[c][3].AddSample(sampleBlock[i]);

                hiGoertz[c][0] = hiGoertz[c][0].AddSample(sampleBlock[i]);
                hiGoertz[c][1] = hiGoertz[c][1].AddSample(sampleBlock[i]);
                hiGoertz[c][2] = hiGoertz[c][2].AddSample(sampleBlock[i]);
                hiGoertz[c][3] = hiGoertz[c][3].AddSample(sampleBlock[i]);
            }
        }

        private static PhoneKey[] Detect(
            IReadOnlyList<IReadOnlyList<Goertzel>> loGoertz,
            IReadOnlyList<IReadOnlyList<Goertzel>> hiGoertz,
            double threshold,
            int numChannels) {
            var phoneKeys = new PhoneKey[numChannels];

            for (var c = 0; c < numChannels; c++) {
                phoneKeys[c] = Detect(loGoertz[c], hiGoertz[c], threshold);
            }

            return phoneKeys;
        }

        private static PhoneKey Detect(IReadOnlyList<Goertzel> loGoertz, IReadOnlyList<Goertzel> hiGoertz, double threshold) {
            var (fstLoIdx, sndLoIdx) = FindMaxTwo(loGoertz);
            var (fstLoVal, sndLoVal) = (loGoertz[fstLoIdx].NormResponse, loGoertz[sndLoIdx].NormResponse);

            var (fstHiIdx, sndHiIdx) = FindMaxTwo(hiGoertz);
            var (fstHiVal, sndHiVal) = (hiGoertz[fstHiIdx].NormResponse, hiGoertz[sndHiIdx].NormResponse);

            //Console.WriteLine($"lo: {fstLoIdx}: {fstLoVal,8:N3}, {sndLoIdx}: {sndLoVal,8:N3}  |  hi: {fstHiIdx}: {fstHiVal,8:N3}, {sndHiIdx}: {sndHiVal,8:N3}");

            return fstLoVal < threshold || fstHiVal < threshold
                || fstLoVal > threshold && sndLoVal > threshold
                || fstHiVal > threshold && sndHiVal > threshold
                || double.IsNaN(fstLoVal) || double.IsNaN(fstHiVal)
                ? PhoneKey.None
                : (highTones[fstHiIdx], lowTones[fstLoIdx]).ToPhoneKey();
        }

        private static (int fstIdx, int sndIdx) FindMaxTwo(IReadOnlyList<Goertzel> goertz) {
            int fst = 0, snd = 1;

            if (goertz[1].NormResponse > goertz[0].NormResponse) {
                snd = 0;
                fst = 1;
            }

            if (goertz[2].NormResponse > goertz[fst].NormResponse) {
                snd = fst;
                fst = 2;
            } else if (goertz[2].NormResponse > goertz[snd].NormResponse) {
                snd = 2;
            }

            if (goertz[3].NormResponse > goertz[fst].NormResponse) {
                snd = fst;
                fst = 3;
            } else if (goertz[3].NormResponse > goertz[snd].NormResponse) {
                snd = 3;
            }

            return (fst, snd);
        }
    }
}

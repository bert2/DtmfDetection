﻿namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Analyzer {
        private readonly float[] buffer;
        private readonly ISamples samples;
        private readonly int blockSize;
        private readonly Detector detector;
        private IReadOnlyList<PhoneKey> prevKeys;

        public Analyzer(ISamples samples, int blockSize = 205) {
            this.samples = samples;
            this.blockSize = blockSize * samples.Channels;
            buffer = new float[this.blockSize];
            detector = new Detector(samples.Channels);
            prevKeys = Enumerable.Repeat(PhoneKey.None, samples.Channels).ToArray();
        }

        public bool CanAnalyze { get; private set; } = true;

        public List<DtmfChange> AnalyzeNextBlock() {
            var currPos = samples.Position;
            var n = samples.ReadNextBlock(buffer, blockSize);

            CanAnalyze = n >= blockSize;

            var currKeys = detector.Detect(buffer.AsSpan().Slice(0, n));
            var changes = FindDtmfChanges(currKeys, prevKeys, currPos, samples.Channels);
            if (!CanAnalyze) changes.AddRange(FindCutOff(currKeys, samples.Position, samples.Channels));

            prevKeys = currKeys;

            return changes;
        }

        private static List<DtmfChange> FindDtmfChanges(
            IReadOnlyList<PhoneKey> currKeys,
            IReadOnlyList<PhoneKey> prevKeys,
            TimeSpan currPos,
            int numChannels) {
            var changes = new List<DtmfChange>(2 * numChannels);

            for (var c = 0; c < numChannels; c++) {
                switch (prevKeys[c], currKeys[c]) {
                    case (PhoneKey.None, PhoneKey.None):
                        break;
                    case (PhoneKey.None, var curr):
                        changes.Add(DtmfChange.Start(curr, currPos, c));
                        break;
                    case (var prev, PhoneKey.None):
                        changes.Add(DtmfChange.Stop(prev, currPos, c));
                        break;
                    case (var prev, var curr) when prev != curr:
                        changes.Add(DtmfChange.Stop(prev, currPos, c));
                        changes.Add(DtmfChange.Start(curr, currPos, c));
                        break;
                }
            }

            return changes;
        }

        private static List<DtmfChange> FindCutOff(
            IReadOnlyList<PhoneKey> prevKeys,
            TimeSpan stopPos,
            int numChannels)
            => FindDtmfChanges(
                Enumerable.Repeat(PhoneKey.None, numChannels).ToArray(),
                prevKeys,
                stopPos,
                numChannels);
    }
}
namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SampleProcessor {
        private readonly float[] buffer;
        private readonly ISamples samples;
        private readonly int numChannels;
        private readonly int blockSize;
        private readonly Detector detector;
        private IReadOnlyList<PhoneKey> previousKeys;
        private int numSamplesRead;

        public SampleProcessor(ISamples samples, int numChannels = 1, int blockSize = 205) {
            this.samples = samples;
            this.numChannels = numChannels;
            this.blockSize = blockSize * numChannels;
            buffer = new float[this.blockSize];
            detector = new Detector(numChannels);
            previousKeys = Enumerable.Repeat(PhoneKey.None, numChannels).ToArray();
            numSamplesRead = this.blockSize; // Optimistically assume that we are going to read at least one sample block.
        }

        public bool CanRead => numSamplesRead >= blockSize;

        public IReadOnlyList<DtmfChange> ProcessNext() {
            numSamplesRead = samples.Read(buffer, blockSize);
            var currentKeys = detector.Analyze(buffer.AsSpan().Slice(0, numSamplesRead));
            var changes = DetectDtmfChanges(currentKeys, previousKeys, numChannels);
            previousKeys = currentKeys;
            return changes;
        }

        private static IReadOnlyList<DtmfChange> DetectDtmfChanges(
            IReadOnlyList<PhoneKey> currentKeys,
            IReadOnlyList<PhoneKey> previousKeys,
            int numChannels) {
            var changes = new List<DtmfChange>(2 * numChannels);

            for (var c = 0; c < numChannels; c++) {
                switch (previousKeys[c], currentKeys[c]) {
                    case (PhoneKey.None, PhoneKey.None):
                        break;
                    case (PhoneKey.None, var curr):
                        changes.Add(DtmfChange.Start(curr, c));
                        break;
                    case (var prev, PhoneKey.None):
                        changes.Add(DtmfChange.Stop(prev, c));
                        break;
                    case (var prev, var curr) when prev != curr:
                        changes.Add(DtmfChange.Stop(prev, c));
                        changes.Add(DtmfChange.Start(curr, c));
                        break;
                }
            }

            return changes;
        }
    }
}

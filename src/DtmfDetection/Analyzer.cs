namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection.Interfaces;

    public class Analyzer : IAnalyzer {
        private readonly float[] buffer;
        private readonly ISamples samples;
        private readonly int blockSize;
        private readonly IDetector detector;
        private IReadOnlyList<PhoneKey> prevKeys;

        public Analyzer(ISamples samples, IDetector detector) {
            if (samples is null) throw new ArgumentNullException(nameof(samples));
            if (detector is null) throw new ArgumentNullException(nameof(detector));
            if (samples.Channels != detector.Channels) throw new InvalidOperationException("'ISamples.Channels' does not match 'Detector.Channels'");
            if (samples.SampleRate != detector.Config.SampleRate) throw new InvalidOperationException("'ISamples.SampleRate' does not match 'Detector.Config.SampleRate'");

            this.samples = samples;
            blockSize = detector.Config.SampleBlockSize * samples.Channels;
            buffer = new float[blockSize];
            this.detector = detector;
            prevKeys = Enumerable.Repeat(PhoneKey.None, samples.Channels).ToArray();
        }

        public static Analyzer Create(ISamples samples, IDetector detector) => new Analyzer(samples, detector);

        public static Analyzer Create(ISamples samples, in Config config) {
            if (samples is null) throw new ArgumentNullException(nameof(samples));
            if (samples.SampleRate != config.SampleRate) throw new InvalidOperationException("'ISamples.SampleRate' does not match 'Config.SampleRate'");

            return new Analyzer(samples, new Detector(samples.Channels, config));
        }

        public bool MoreSamplesAvailable { get; private set; } = true;

        public IList<DtmfChange> AnalyzeNextBlock() {
            var currPos = samples.Position;
            var n = samples.Read(buffer, blockSize);

            MoreSamplesAvailable = n >= blockSize;

            var currKeys = detector.Detect(buffer.AsSpan().Slice(0, n));
            var changes = FindDtmfChanges(currKeys, prevKeys, currPos, samples.Channels);
            if (!MoreSamplesAvailable) changes.AddRange(FindCutOff(currKeys, samples.Position, samples.Channels));

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

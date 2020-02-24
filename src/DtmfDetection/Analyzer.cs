namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection.Interfaces;

    /// <summary>The `Analyzer` reads sample blocks of size `Config.SampleBlockSize *  * ISamples.Channels` from the input sample
    /// data and feeds each sample block to its `IDetector` every time `AnalyzeNextBlock()` is called. An internal state machine
    /// is used to skip redundant reports of the same DTMF tone detected in consecutive sample blocks. Instead only the starting
    /// and stop position of each DMTF tone will be reported. When no more samples are available the property `MoreSamplesAvailable`
    /// will be set to `false` and the analysis is considered finished (i.e. subsequent calls to `AnalyzeNextBlock()` should be
    /// avoided as they might fail or result in undefined behavior, depending on the `ISamples` implementation.</summary>
    public class Analyzer : IAnalyzer {
        private readonly float[] buffer;
        private readonly ISamples samples;
        private readonly int blockSize;
        private readonly IDetector detector;
        private IReadOnlyList<PhoneKey> prevKeys;

        /// <summary>Creates a new `Analyzer` that will feed the given sample data to the given `IDetector`.</summary>
        /// <param name="samples">The samples to analyze. Its number of channels must match the number of channels
        /// the `IDetector` has been created for. Its sample rate must match the sample rate of the `IDetector`s config.</param>
        /// <param name="detector">The detector to use for the analysis.</param>
        public Analyzer(ISamples samples, IDetector detector) {
            _ = samples ?? throw new ArgumentNullException(nameof(samples));
            _ = detector ?? throw new ArgumentNullException(nameof(detector));
            if (samples.Channels != detector.Channels) throw new InvalidOperationException("'ISamples.Channels' does not match 'Detector.Channels'");
            if (samples.SampleRate != detector.Config.SampleRate) throw new InvalidOperationException("'ISamples.SampleRate' does not match 'Detector.Config.SampleRate'");

            this.samples = samples;
            blockSize = detector.Config.SampleBlockSize * samples.Channels;
            buffer = new float[blockSize];
            this.detector = detector;
            prevKeys = Enumerable.Repeat(PhoneKey.None, samples.Channels).ToArray();
        }

        /// <summary>Creates a new `Analyzer` that will feed the given sample data to the given `IDetector`.</summary>
        /// <param name="samples">The samples to analyze. Its number of channels must match the number of channels
        /// the `IDetector` has been created for. Its sample rate must match the sample rate of the `IDetector`s config.</param>
        /// <param name="detector">The detector to use for the analysis.</param>
        /// <returns>A new `Analyzer` instance.</returns>
        public static Analyzer Create(ISamples samples, IDetector detector) => new Analyzer(samples, detector);

        /// <summary>Creates a new `Analyzer` using a self-created instance of `Detector` to feed the given sample data
        /// to it.</summary>
        /// <param name="samples">The samples to analyze. Its sample rate must match the sample rate of the given `Config`.</param>
        /// <param name="config">The detector config used to create a `Detector`.</param>
        /// <returns>A new `Analyzer` instance.</returns>
        public static Analyzer Create(ISamples samples, in Config config) {
            if (samples is null) throw new ArgumentNullException(nameof(samples));
            if (samples.SampleRate != config.SampleRate) throw new InvalidOperationException("'ISamples.SampleRate' does not match 'Config.SampleRate'");

            return new Analyzer(samples, new Detector(samples.Channels, config));
        }

        /// <summary>Indicates whether there is more data to analyze. `AnalyzeNextBlock()` should not be called when this is `false`.
        /// Is `true` initially and turns `false` as soon as `ISamples.Read()` returned a number less
        /// than `Config.SampleBlockSize`.</summary>
        public bool MoreSamplesAvailable { get; private set; } = true;

        /// <summary>Tries to read `Config.SampleBlockSize * ISamples.Channels` samples from the input data and runs DTMF detection on
        /// that sample block. Should only be called when `MoreSamplesAvailable` is true.</summary>
        /// <returns>A list of the detected `DtmfChange`s representing DTMF tones that started or stopped in the analyzed sample
        /// block.</returns>
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
            int channels) {
            var changes = new List<DtmfChange>(2 * channels);

            for (var c = 0; c < channels; c++) {
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
            int channels)
            => FindDtmfChanges(
                Enumerable.Repeat(PhoneKey.None, channels).ToArray(),
                prevKeys,
                stopPos,
                channels);
    }
}

namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Provides helpers to generate a sequence of `DtmfTone`s from a list of `DtmfChange`s.</summary>
    public static class ToDtmfTonesExt {
        /// <summary>Converts a list of `DtmfChange`s to a sequence of `DtmfTone`s by finding the matching stop of a DTMF tones
        /// to each start of a DTMF tone and merging both into a single `DtmfTone` struct.</summary>
        /// <param name="dtmfs">A list of `DtmfChange`s ordered by `DtmfChange.Position` in ascending order.
        /// The list must be consistent (i.e. there must be a "DTMF stop" after every "DTMF start" somehwere in the list) otherwise
        /// an `InvalidOperationException` will be thrown.</param>
        /// <returns>A sequence of `DtmfTone`s orderd by `DtmfTone.Position` in ascending order.</returns>
        public static IEnumerable<DtmfTone> ToDtmfTones(this IList<DtmfChange> dtmfs) => dtmfs
            .Select((dtmf, idx) => (dtmf, idx))
            .Where(x => x.dtmf.IsStart)
            .Select(x => (start: x.dtmf, stop: dtmfs.FindMatchingStop(offset: x.idx + 1, x.dtmf)))
            .Select(x => DtmfTone.From(x.start, x.stop));

        /// <summary>Finds the stop of a DTMF tone matching the given start of a DTMF tone in a list of `DtmfChange`s.
        /// A `DtmfChange x` matches `start` when:
        ///   `x.IsStop == true`,
        ///   `x.Channel == start.Channel`,
        ///   `x.Key == start.Key`,
        ///   and `x.Position >= start.Position`.</summary>
        /// <param name="dtmfs">The list of `DtmfChange`s to search in. Should be ordered by `DtmfChange.Position` in ascending order.</param>
        /// <param name="offset">An offset into the list to start searching from. Useful for optimizing performance.</param>
        /// <param name="start">The DTMF start to find a matching stop for.</param>
        /// <returns>The found stop of the DTMF tone. Throws an `InvalidOperationException` if no matching stop could be found.</returns>
        public static DtmfChange FindMatchingStop(this IList<DtmfChange> dtmfs, int offset, in DtmfChange start) => dtmfs
            .Find(offset, IsStopOf(start))
            ?? throw new InvalidOperationException($"Inconsistent input list. Unable to find end of DTMF tone (start: {start}).");

        private static Predicate<DtmfChange> IsStopOf(in DtmfChange start) {
            var startKey = start.Key;
            var startCh = start.Channel;
            var startPos = start.Position;
            return stop => stop.IsStop
                        && stop.Channel == startCh
                        && stop.Key == startKey
                        && stop.Position >= startPos;
        }

        private static T? Find<T>(this IList<T> xs, int offset, Predicate<T> p) where T : struct {
            for (var i = offset; i < xs.Count; i++) {
                if (p(xs[i])) return xs[i];
            }

            return null;
        }
    }
}

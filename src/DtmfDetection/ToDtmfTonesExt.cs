namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ToDtmfTonesExt {
        public static IEnumerable<DtmfTone> ToDtmfTones(this IList<DtmfChange> xs) => xs
            .Select((dtmf, idx) => (dtmf, idx))
            .Where(x => x.dtmf.IsStart)
            .Select(x => (start: x.dtmf, stop: xs.FindMatchingStop(offset: x.idx + 1, x.dtmf)))
            .Select(x => DtmfTone.From(x.start, x.stop));

        public static DtmfChange FindMatchingStop(this IList<DtmfChange> xs, int offset, in DtmfChange start) => xs
            .Find(offset, IsStopOf(start))
            ?? throw new InvalidOperationException($"Inconsistent input list. Unable to find end of DTMF tone (start: {start}).");

        private static Predicate<DtmfChange> IsStopOf(in DtmfChange start) {
            var startKey = start.Key;
            var startCh = start.Channel;
            return stop => stop.IsStop && stop.Channel == startCh && stop.Key == startKey;
        }

        private static T? Find<T>(this IList<T> xs, int offset, Predicate<T> predicate) where T : struct {
            for (var i = offset; i < xs.Count; i++) {
                if (predicate(xs[i])) return xs[i];
            }

            return null;
        }
    }
}

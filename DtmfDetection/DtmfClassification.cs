namespace DtmfDetection
{
    using System;
    using System.Collections.Generic;

    public static class DtmfClassification
    {
        private static readonly Dictionary<Tuple<int, int>, DtmfTone> DtmfTones = new Dictionary<Tuple<int, int>, DtmfTone>
        { 
            [KeyOf(DtmfTone.One)]   = DtmfTone.One,   [KeyOf(DtmfTone.Two)]   = DtmfTone.Two,   [KeyOf(DtmfTone.Three)] = DtmfTone.Three, [KeyOf(DtmfTone.A)] = DtmfTone.A,
            [KeyOf(DtmfTone.Four)]  = DtmfTone.Four,  [KeyOf(DtmfTone.Five)]  = DtmfTone.Five,  [KeyOf(DtmfTone.Six)]   = DtmfTone.Six,   [KeyOf(DtmfTone.B)] = DtmfTone.B,
            [KeyOf(DtmfTone.Seven)] = DtmfTone.Seven, [KeyOf(DtmfTone.Eight)] = DtmfTone.Eight, [KeyOf(DtmfTone.Nine)]  = DtmfTone.Nine,  [KeyOf(DtmfTone.C)] = DtmfTone.C,
            [KeyOf(DtmfTone.Star)]  = DtmfTone.Star,  [KeyOf(DtmfTone.Zero)]  = DtmfTone.Zero,  [KeyOf(DtmfTone.Hash)]  = DtmfTone.Hash,  [KeyOf(DtmfTone.D)] = DtmfTone.D
        };

        public static DtmfTone For(int highTone, int lowTone)
        {
            DtmfTone tone;
            return DtmfTones.TryGetValue(KeyOf(highTone, lowTone), out tone)
                       ? tone
                       : DtmfTone.None;
        }

        private static Tuple<int, int> KeyOf(DtmfTone t) => KeyOf(t.HighTone, t.LowTone);

        private static Tuple<int, int> KeyOf(int highTone, int lowTone) => Tuple.Create(highTone, lowTone);
    }
}
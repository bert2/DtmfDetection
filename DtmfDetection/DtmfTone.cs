namespace DtmfDetection
{
    using System;

    public struct DtmfTone : IEquatable<DtmfTone>
    {
        private DtmfTone(int highTone, int lowTone, PhoneKey key)
        {
            HighTone = highTone;
            LowTone = lowTone;
            Key = key;
        }

        #region DTMF Tone definitions

        public static DtmfTone None { get; } = new DtmfTone(0, 0, PhoneKey.None);

        public static DtmfTone Zero { get; } = new DtmfTone(1336, 941, PhoneKey.Zero);

        public static DtmfTone One { get; } = new DtmfTone(1209, 697, PhoneKey.One);

        public static DtmfTone Two { get; } = new DtmfTone(1336, 697, PhoneKey.Two);

        public static DtmfTone Three { get; } = new DtmfTone(1477, 697, PhoneKey.Three);

        public static DtmfTone Four { get; } = new DtmfTone(1209, 770, PhoneKey.Four);

        public static DtmfTone Five { get; } = new DtmfTone(1336, 770, PhoneKey.Five);

        public static DtmfTone Six { get; } = new DtmfTone(1477, 770, PhoneKey.Six);

        public static DtmfTone Seven { get; } = new DtmfTone(1209, 852, PhoneKey.Seven);

        public static DtmfTone Eight { get; } = new DtmfTone(1336, 852, PhoneKey.Eight);

        public static DtmfTone Nine { get; } = new DtmfTone(1477, 852, PhoneKey.Nine);

        public static DtmfTone Star { get; } = new DtmfTone(1209, 941, PhoneKey.Star);

        public static DtmfTone Hash { get; } = new DtmfTone(1477, 941, PhoneKey.Hash);

        public static DtmfTone A { get; } = new DtmfTone(1633, 697, PhoneKey.A);

        public static DtmfTone B { get; } = new DtmfTone(1633, 770, PhoneKey.B);

        public static DtmfTone C { get; } = new DtmfTone(1633, 852, PhoneKey.C);

        public static DtmfTone D { get; } = new DtmfTone(1633, 941, PhoneKey.D);

        #endregion DTMF Tone definitions

        public PhoneKey Key { get; }

        public int HighTone { get; }

        public int LowTone { get; }

        public override string ToString() => Key.ToString();

        #region Equality implementations

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj is DtmfTone)
                return Equals((DtmfTone)obj);

            return false;
        }

        public bool Equals(DtmfTone other) => Key == other.Key;

        public override int GetHashCode() => Key.GetHashCode();

        public static bool operator ==(DtmfTone a, DtmfTone b) => a.Equals(b);

        public static bool operator !=(DtmfTone a, DtmfTone b) => !(a == b);

        #endregion
    }
}

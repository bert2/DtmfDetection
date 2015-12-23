namespace DtmfDetection
{
    public class DtmfTone
    {
        #region DTMF tone definitions

        public static readonly DtmfTone None     = new DtmfTone(0,      0, PhoneKey.None);

        public static readonly DtmfTone Zero     = new DtmfTone(1336, 941, PhoneKey.Zero);

        public static readonly DtmfTone One      = new DtmfTone(1209, 697, PhoneKey.One);

        public static readonly DtmfTone Two      = new DtmfTone(1336, 697, PhoneKey.Two);

        public static readonly DtmfTone Three    = new DtmfTone(1477, 697, PhoneKey.Three);

        public static readonly DtmfTone Four     = new DtmfTone(1209, 770, PhoneKey.Four);

        public static readonly DtmfTone Five     = new DtmfTone(1336, 770, PhoneKey.Five);

        public static readonly DtmfTone Six      = new DtmfTone(1477, 770, PhoneKey.Six);

        public static readonly DtmfTone Seven    = new DtmfTone(1209, 852, PhoneKey.Seven);

        public static readonly DtmfTone Eight    = new DtmfTone(1336, 852, PhoneKey.Eight);

        public static readonly DtmfTone Nine     = new DtmfTone(1477, 852, PhoneKey.Nine);

        public static readonly DtmfTone Asterisk = new DtmfTone(1209, 941, PhoneKey.Asterisk);

        public static readonly DtmfTone Hash     = new DtmfTone(1477, 941, PhoneKey.Hash);

        public static readonly DtmfTone A        = new DtmfTone(1633, 697, PhoneKey.A);

        public static readonly DtmfTone B        = new DtmfTone(1633, 770, PhoneKey.B);

        public static readonly DtmfTone C        = new DtmfTone(1633, 852, PhoneKey.C);

        public static readonly DtmfTone D        = new DtmfTone(1633, 941, PhoneKey.D);

        #endregion DTMF tone definitions

        private DtmfTone(int highTone, int lowTone, PhoneKey key)
        {
            HighTone = highTone;
            LowTone = lowTone;
            Key = key;
        }

        public PhoneKey Key { get; }

        public int HighTone { get; }

        public int LowTone { get; }

        public override string ToString()
        {
            return Key.ToString();
        }
    }
}

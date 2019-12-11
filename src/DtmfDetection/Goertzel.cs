namespace DtmfDetection {
    using System;
    using System.Globalization;

    public readonly struct Goertzel : IEquatable<Goertzel> {
        private readonly double C, S1, S2, E;

        private Goertzel(double c, double s1, double s2, double e) => (C, S1, S2, E) = (c, s1, s2, e);

        public static Goertzel Init(int targetFreq, int sampleRate, int numSamples) {
            var k = Math.Round((double)targetFreq / sampleRate * numSamples);
            var c = 2.0 * Math.Cos(2.0 * Math.PI * k / numSamples);
            return new Goertzel(c, .0, .0, .0);
        }

        public double Response => S1 * S1 + S2 * S2 - S1 * S2 * C;

        public double NormResponse => Response / E;

        public Goertzel AddSample(float sample) => new Goertzel(
            c: C,
            s1: sample + C * S1 - S2,
            s2: S1,
            e: E + sample * sample);

        public Goertzel Reset() => new Goertzel(c: C, s1: 0, s2: 0, e: 0);

        public override string ToString() => NormResponse.ToString(CultureInfo.InvariantCulture);

        #region Equality implementations

        public bool Equals(Goertzel other) => C == other.C && S1 == other.S1 && S2 == other.S2 && E == other.E;

        public override bool Equals(object? obj) => obj is Goertzel g && Equals(g);

        public static bool operator ==(Goertzel left, Goertzel right) => left.Equals(right);

        public static bool operator !=(Goertzel left, Goertzel right) => !(left == right);

        public override int GetHashCode() => HashCode.Combine(C, S1, S2, E);

        #endregion Equality implementations
    }
}

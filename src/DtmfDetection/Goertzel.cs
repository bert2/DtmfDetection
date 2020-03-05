namespace DtmfDetection {
    using System;
    using System.Globalization;

    /// <summary>The actual implementation of the Goertzel algorithm (https://en.wikipedia.org/wiki/Goertzel_algorithm) that estimates the strength of a frequency in a signal. It works similar to a Fourier transform except that it doesn't analyze the whole spectrum, but only a single frequency.</summary>
    public readonly struct Goertzel : IEquatable<Goertzel> {
        /// <summary>Stores a pre-computed coefficient calculated from the parameters of `Init()`.</summary>
        public readonly double C;

        /// <summary>Stores the state of the `Goertzel`. Used to determine the strength of the target frequency in the signal.</summary>
        public readonly double S1, S2;

        /// <summary>Accumulates the total signal energy of the signal. Used for normalization.</summary>
        public readonly double E;

        /// <summary>Used to create a new `Goertzel` from the values of a previous one.</summary>
        /// <param name="c">The pre-computed coefficient.</param>
        /// <param name="s1">The `Goertzel` state.</param>
        /// <param name="s2">The `Goertzel` state.</param>
        /// <param name="e">The total signal energy accumulated so far.</param>
        public Goertzel(double c, double s1, double s2, double e) => (C, S1, S2, E) = (c, s1, s2, e);

        /// <summary>Initializes a `Goertzel` for a given target frequency.</summary>
        /// <param name="targetFreq">The target frequency to estimate the strength for in a signal.</param>
        /// <param name="sampleRate">The sample rate of the signal. A rate of `8000` (Hz) is recommended.</param>
        /// <param name="numSamples">The number of samples that will be added to the `Goertzel` before `Response` or `NormResponse` are queried. It is recommended to use a value of `205` as this minimizes errors.</param>
        /// <returns>A new `Goertzel` with a pre-computed coefficient.</returns>
        public static Goertzel Init(int targetFreq, int sampleRate, int numSamples) {
            var k = Math.Round((double)targetFreq / sampleRate * numSamples);
            var c = 2.0 * Math.Cos(2.0 * Math.PI * k / numSamples);
            return new Goertzel(c, .0, .0, .0);
        }

        /// <summary>Calculates and returns the estimated strength of the frequency in the samples given so far.</summary>
        public double Response => S1 * S1 + S2 * S2 - S1 * S2 * C;

        /// <summary>Calculates `Response`, but normalized with the total signal energy, which achieves loudness invariance.</summary>
        public double NormResponse => Response / E;

        /// <summary>Adds a new sample to this `Goertzel` and returns a new one created from the previous `Goertzel` values and the sample.</summary>
        /// <param name="sample">The sample value to add.</param>
        /// <returns>A new `Goertzel` that has the sample value added to this one.</returns>
        public Goertzel AddSample(float sample) => new Goertzel(
            c: C,
            s1: sample + C * S1 - S2,
            s2: S1,
            e: E + sample * sample);

        /// <summary>Creates a new `Goertzel` from this one's coefficient `C`, but resets the state (`S1`, `S2`) and the total signal energy (`E`) to `0`. Useful to save the computation of `C` when the parameters of `Init()` were to stay the same.</summary>
        /// <returns>A new `Goertzel` with this one's value of `C`.</returns>
        public Goertzel Reset() => new Goertzel(c: C, s1: 0, s2: 0, e: 0);

        /// <summary>Prints the value of `NormResponse` to a `string` and returns it.</summary>
        /// <returns>The `NormResponse` of this `Goertzel` as a `string`.</returns>
        public override string ToString() => NormResponse.ToString(CultureInfo.InvariantCulture);

        #region Equality implementations

        /// <summary>Indicates whether the current `Goertzel` is equal to another `Goertzel`.</summary>
        /// <param name="other">A `Goertzel` to compare with this `Goertzel`.</param>
        /// <returns>Returns `true` if the current `Goertzel` is equal to `other`; otherwise, `false`.</returns>
        public bool Equals(Goertzel other) => (C, S1, S2, E) == (other.C, other.S1, other.S2, other.E);

        /// <summary>Indicates whether this `Goertzel` and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current `Goertzel`.</param>
        /// <returns>Returns `true` if `obj` this `Goertzel` are the same type and represent the same value; otherwise, `false`.</returns>
        public override bool Equals(object? obj) => obj is Goertzel other && Equals(other);

        /// <summary>Indicates whether the left-hand side `Goertzel` is equal to the right-hand side `Goertzel`.</summary>
        /// <param name="left">The left-hand side `Goertzel` of the comparison.</param>
        /// <param name="right">The right-hand side `Goertzel` of the comparison.</param>
        /// <returns>Returns `true` if the left-hand side `Goertzel` is equal to the right-hand side `Goertzel`; otherwise, `false`.</returns>
        public static bool operator ==(Goertzel left, Goertzel right) => left.Equals(right);

        /// <summary>Indicates whether the left-hand side `Goertzel` is not equal to the right-hand side `Goertzel`.</summary>
        /// <param name="left">The left-hand side `Goertzel` of the comparison.</param>
        /// <param name="right">The right-hand side `Goertzel` of the comparison.</param>
        /// <returns>Returns `true` if the left-hand side `Goertzel` is not equal to the right-hand side `Goertzel`; otherwise, `false`.</returns>
        public static bool operator !=(Goertzel left, Goertzel right) => !(left == right);

        /// <summary>Returns the hash code for this `Goertzel`.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this `Goertzel`.</returns>
        public override int GetHashCode() => HashCode.Combine(C, S1, S2, E);

        #endregion Equality implementations
    }
}

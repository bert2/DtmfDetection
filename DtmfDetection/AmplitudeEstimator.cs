namespace DtmfDetection
{
    using System;

    public class AmplitudeEstimator
    {
        private readonly double c;

        private double s1;

        private double s2;

        private Lazy<double> amplitude;

        public AmplitudeEstimator(double targetFrequency, double sampleRate, int numberOfSamples)
        {
            Reset();
            var k = targetFrequency / sampleRate * numberOfSamples;
            c = 2.0 * Math.Cos(2.0 * Math.PI * k / numberOfSamples);
        }

        public double Amplitude => amplitude.Value;

        public void Add(float sample)
        {
            var s0 = sample + c * s1 - s2;
            s2 = s1;
            s1 = s0;
        }

        public void Reset()
        {
            s1 = s2 = .0;
            amplitude = new Lazy<double>(() => Math.Sqrt(s1*s1 + s2*s2 - s1 * s2 * c));
        }
    }
}
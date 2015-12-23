namespace DtmfDetection
{
    using System;

    public class FourierTransform
    {
        private readonly double c;

        public FourierTransform(double targetFrequency, double sampleRate, int numberOfSamples)
        {
            TargetFrequency = targetFrequency;
            SampleRate = sampleRate;
            NumberOfSamples = numberOfSamples;

            var k = TargetFrequency / SampleRate * NumberOfSamples;
            c = 2.0 * Math.Cos(2.0 * Math.PI * k / NumberOfSamples);
        }

        public double TargetFrequency { get; }

        public double SampleRate { get; }

        public int NumberOfSamples { get; }

        public double? LastAmplitude { get; private set; }

        public double AmplitudeIn(float[] samples)
        {
            double s1 = .0, s2 = .0;

            for (var i = 0; i < NumberOfSamples; i++)
            {
                var s0 = samples[i] + c * s1 - s2;
                s2 = s1;
                s1 = s0;
            }
            
            LastAmplitude = Math.Sqrt(s1*s1 + s2*s2 - s1 * s2 * c);
            return LastAmplitude.Value;
        }
    }
}
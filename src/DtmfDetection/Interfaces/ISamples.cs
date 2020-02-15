namespace DtmfDetection.Interfaces {
    using System;

    public interface ISamples {
        int Channels { get; }

        int SampleRate { get; }

        TimeSpan Position { get; }

        int Read(float[] buffer, int count);
    }
}

namespace DtmfDetection.Interfaces {
    using System;

    public interface ISamples {
        int Channels { get; }

        int SampleRate { get; }

        TimeSpan Position { get; }

        int ReadNextBlock(float[] buffer, int count);
    }
}

namespace DtmfDetection.LastRelease {
    using System.Collections.Generic;

    public interface ISampleSource
    {
        bool HasSamples { get; }

        int SampleRate { get; }

        int Channels { get; }

        IEnumerable<float>  Samples { get; }
    }
}
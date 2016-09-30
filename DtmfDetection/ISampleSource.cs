using System.Collections.Generic;

namespace DtmfDetection
{
    public interface ISampleSource
    {
        bool HasSamples { get; }

        int SampleRate { get; }

        IEnumerable<float>  Samples { get; }
    }
}
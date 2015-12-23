using System.Collections.Generic;

namespace DtmfDetection
{
    public interface ISampleSource
    {
        bool HasSamples { get; }

        IEnumerable<float>  Samples { get; }
    }
}
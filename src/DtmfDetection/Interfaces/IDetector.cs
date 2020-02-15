namespace DtmfDetection.Interfaces {
    using System;
    using System.Collections.Generic;

    public interface IDetector {
        int Channels { get; }

        Config Config { get; }

        IReadOnlyList<PhoneKey> Detect(in ReadOnlySpan<float> sampleBlock);
    }
}

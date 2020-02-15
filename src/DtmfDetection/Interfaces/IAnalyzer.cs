namespace DtmfDetection.Interfaces {
    using System.Collections.Generic;

    public interface IAnalyzer {
        bool MoreSamplesAvailable { get; }

        IList<DtmfChange> AnalyzeNextBlock();
    }
}

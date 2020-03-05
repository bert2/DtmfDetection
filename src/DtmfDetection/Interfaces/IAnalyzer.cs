namespace DtmfDetection.Interfaces {
    using System.Collections.Generic;

    /// <summary>Interface to decouple the `BackgroundAnalyzer` from the `Analyzer` it is using by default. Use this if you want to inject your own analyzer into the `BackgroundAnalyzer`. Feel free to start by copying the original `Analyzer` and adjust it to your needs.</summary>
    public interface IAnalyzer {
        /// <summary>Indicates whether there is more data to analyze. Should always be `true` initially and once it turned `false`, it should never turn back to `true` again.</summary>
        bool MoreSamplesAvailable { get; }

        /// <summary>Analyzes the next block of samples. The size of the analyzed block should match `Config.SampleBlockSize` multiplied by the number of channels in the sample data. This might throw when called while `MoreSamplesAvailable` is `false`.</summary>
        /// <returns>A list of detected DTMF changes.</returns>
        IList<DtmfChange> AnalyzeNextBlock();
    }
}

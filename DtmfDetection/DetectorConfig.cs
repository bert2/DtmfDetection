namespace DtmfDetection
{
    public class DetectorConfig
    {
        public int MaxSampleRate { get; } = 8000;

        // Using 205 samples minimizes error (distance of DTMF frequency to center of DFT bin).
        public int SampleBlockSize { get; } = 205;

        public double PowerThreshold { get; } = 100.0;
    }
}
namespace DtmfDetection
{
    public class AmplitudeEstimatorFactory
    {
        private readonly int sampleRate;

        private readonly int sampleBlockSize;

        public AmplitudeEstimatorFactory(int sampleRate, int sampleBlockSize)
        {
            this.sampleRate = sampleRate;
            this.sampleBlockSize = sampleBlockSize;
        }

        public AmplitudeEstimator CreateFor(int tone) => new AmplitudeEstimator(tone, sampleRate, sampleBlockSize);
    }
}
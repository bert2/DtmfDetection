namespace DtmfDetection.NAudio
{
    using global::NAudio.Wave;

    public class SampleBlockProvider
    {
        private readonly ISampleProvider source;

        public SampleBlockProvider(ISampleProvider source, int blockSize)
        {
            this.source = source;
            BlockSize = blockSize;
            CurrentBlock = new float[blockSize];
        }

        public int BlockSize { get; }

        public float[] CurrentBlock { get; }

        public int ReadNextBlock() => source.Read(CurrentBlock, 0, BlockSize);
    }
}
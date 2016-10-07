namespace DtmfDetection.NAudio
{
    using global::NAudio.Wave;

    public class SampleBlockProvider
    {
        private readonly ISampleProvider source;

        public SampleBlockProvider(ISampleProvider source, int blockSize)
        {
            this.source = source;
            BlockSize = blockSize * Channels;
            CurrentBlock = new float[BlockSize];
        }

        public int SampleRate => source.WaveFormat.SampleRate;

        public int BlockSize { get; }

        public int Channels => source.WaveFormat.Channels;

        public float[] CurrentBlock { get; }

        public int ReadNextBlock() => source.Read(CurrentBlock, 0, BlockSize);
    }
}
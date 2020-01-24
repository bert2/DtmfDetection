namespace DtmfDetection.NAudio.LastRelease {
    using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
        public float[] CurrentBlock { get; }

        public int ReadNextBlock() => source.Read(CurrentBlock, 0, BlockSize);
    }
}
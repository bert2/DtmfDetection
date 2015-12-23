using System;

namespace DtmfDetection.NAudio
{
    using global::NAudio.Wave;

    public class StreamingDtmfAudio
    {
        private readonly DtmfDetector dtmfDetector = new DtmfDetector();

        private readonly BufferedWaveProvider sourceBuffer;

        private readonly SampleBlockProvider samples;

        private readonly TimeSpan sampleBlockDuration;

        public StreamingDtmfAudio(BufferedWaveProvider source)
        {
            sourceBuffer = source;
            samples = source.AsMono()
                            .SampleWith(DtmfDetector.SampleRate)
                            .Blockwise(DtmfDetector.SampleBlockSize);
            sampleBlockDuration = TimeSpan.FromSeconds(DtmfDetector.SampleBlockSize / (double)DtmfDetector.SampleRate);
        }

        public DtmfTone LastDtmfTone { get; private set; } = DtmfTone.None;

        public void WaitForDtmfTone()
        {
            while (true)
            {
                sourceBuffer.WaitForData(sampleBlockDuration);
                samples.ReadNextBlock();

                LastDtmfTone = dtmfDetector.FindDtmfTone(samples.CurrentBlock);

                if (LastDtmfTone != DtmfTone.None)
                    return;
            }
        }

        public void WaitForEndOfDtmfTone()
        {
            while (true)
            {
                sourceBuffer.WaitForData(sampleBlockDuration);
                samples.ReadNextBlock();

                var nextDtmfTone = dtmfDetector.FindDtmfTone(samples.CurrentBlock);

                if (nextDtmfTone == LastDtmfTone)
                    continue;

                return;
            }
        }
    }
}
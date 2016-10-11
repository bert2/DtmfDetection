namespace DtmfDetection.NAudio
{
    using System;

    using global::NAudio.Utils;
    using global::NAudio.Wave;

    public class MonoSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;

        private readonly int sourceChannels;

        private float[] sourceBuffer;

        public MonoSampleProvider(ISampleProvider sourceProvider)
        {
            this.sourceProvider = sourceProvider;
            sourceChannels = sourceProvider.WaveFormat.Channels;
            WaveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, sourceProvider.WaveFormat.BitsPerSample, 1);
        }

        public WaveFormat WaveFormat { get; }

        public int Read(float[] buffer, int offset, int count)
        {
            var sourceBytesRequired = count * sourceChannels;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);

            var samplesRead = sourceProvider.Read(sourceBuffer, 0, sourceBytesRequired);
            var sampleFramesRead = samplesRead / sourceChannels;

            for (var sampleIndex = 0; sampleIndex < samplesRead; sampleIndex += sourceChannels)
                buffer[offset++] = Clamp(AverageOfSampleFrame(sampleIndex));

            return sampleFramesRead;
        }

        private float AverageOfSampleFrame(int frameStart)
        {
            var mixedValue = 0.0f;

            for (var channel = 0; channel < sourceChannels; channel++)
                mixedValue += sourceBuffer[frameStart + channel];

            return mixedValue / sourceChannels;
        }

        private static float Clamp(float value) => Math.Max(-1.0f, Math.Min(value, 1.0f));
    }
}
namespace DtmfDetection.NAudio {
    using System;
    using System.Threading;
    using global::NAudio.Wave;

    public static class BufferedWaveProviderExt {
        public static void WaitForSamples(this BufferedWaveProvider source, int count) {
            var bytesPerSample = source.WaveFormat.BitsPerSample / 8 * source.WaveFormat.Channels;
            var bytesPerSampleBlock = bytesPerSample * count;
            var missingBytes = bytesPerSampleBlock - source.BufferedBytes;

            if (missingBytes > 0) {
                var missingSamples = (double)missingBytes / bytesPerSample;
                var waitTime = (int)Math.Round(missingSamples / source.WaveFormat.SampleRate * 1000);
                Thread.Sleep(Math.Max(waitTime, 1));
            }
        }
    }
}

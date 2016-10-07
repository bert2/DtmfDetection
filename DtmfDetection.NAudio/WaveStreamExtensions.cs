namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;
    using System.Linq;

    using global::NAudio.Wave;

    public static class WaveStreamExtensions
    {
        public static IEnumerable<DtmfOccurence> DtmfTones(this WaveStream waveFile, bool forceMono = true)
        {
            var config = new DetectorConfig();
            var dtmfAudio = DtmfAudio.CreateFrom(new StaticSampleSource(config, waveFile, forceMono), config);
            var detectedTones = new Queue<DtmfOccurence>();

            while (detectedTones.Any() 
                || dtmfAudio.Forward(
                    (channel, tone) => waveFile.CurrentTime, 
                    (channel, start, tone) => detectedTones.Enqueue(new DtmfOccurence(tone, channel, start, waveFile.CurrentTime - start))))
            {
                if (detectedTones.Any())
                    yield return detectedTones.Dequeue();
            }

            // Yield any tones that might have been cut off by EOF.
            foreach (var tone in detectedTones)
                yield return tone;
        }
    }
}
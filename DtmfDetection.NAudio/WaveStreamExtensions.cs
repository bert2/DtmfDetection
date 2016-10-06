namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;
    using System.Linq;

    using global::NAudio.Wave;

    public static class WaveStreamExtensions
    {
        public static IEnumerable<DtmfOccurence> DtmfTones(this WaveStream waveFile)
        {
            var config = new DetectorConfig();
            var dtmfAudio = DtmfAudio.CreateFrom(new StaticSampleSource(config, waveFile), config);
            var detectedTones = new Queue<DtmfOccurence>();

            while (detectedTones.Any() 
                || dtmfAudio.Forward(
                    tone => waveFile.CurrentTime, 
                    (start, tone) => detectedTones.Enqueue(new DtmfOccurence(tone, 0, start, waveFile.CurrentTime - start))))
            {
                if (detectedTones.Any())
                    yield return detectedTones.Dequeue();
            }
        }
    }
}
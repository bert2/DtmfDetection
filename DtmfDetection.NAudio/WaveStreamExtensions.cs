namespace DtmfDetection.NAudio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::NAudio.Wave;

    public static class WaveStreamExtensions
    {
        public static IEnumerable<DtmfPosition> DtmfTones(this WaveStream waveFile)
        {
            var config = new DetectorConfig();
            var dtmfAudio = DtmfAudio.CreateFrom(new StaticSampleSource(config, waveFile), config);
            var detectedTones = new Queue<DtmfPosition>();

            while (detectedTones.Any() 
                || dtmfAudio.Forward(
                    tone => waveFile.CurrentTime, 
                    (start, tone) => detectedTones.Enqueue(new DtmfPosition(new DtmfOccurence(tone, 0), start, waveFile.CurrentTime - start))))
            {
                if (detectedTones.Any())
                    yield return detectedTones.Dequeue();
            }
        }
    }
}
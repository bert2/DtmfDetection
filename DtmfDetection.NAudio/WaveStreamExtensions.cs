namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public static class WaveStreamExtensions
    {
        public static IEnumerable<DtmfOccurence> DtmfTones(this WaveStream waveFile)
        {
            var config = new DetectorConfig();
            var dtmfAudio = DtmfAudio.CreateFrom(new StaticSampleSource(config, waveFile), config);

            while (dtmfAudio.Wait() != DtmfTone.None)
            {
                var current = dtmfAudio.CurrentDtmfTone;
                var start = waveFile.CurrentTime;
                dtmfAudio.Skip();
                var duration = waveFile.CurrentTime - start;

                yield return new DtmfOccurence(current, start, duration);
            }
        }
    }
}
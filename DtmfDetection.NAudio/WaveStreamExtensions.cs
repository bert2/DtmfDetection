namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public static class WaveStreamExtensions
    {
        public static IEnumerable<DtmfPosition> DtmfTones(this WaveStream waveFile)
        {
            var config = new DetectorConfig();
            var dtmfAudio = DtmfAudio.CreateFrom(new StaticSampleSource(config, waveFile), config);
            var next = new DtmfOccurence(DtmfTone.None, -1);

            while (next.Tone != DtmfTone.None || dtmfAudio.Wait().Tone != DtmfTone.None)
            {
                var current = dtmfAudio.CurrentDtmfTone;
                var start = waveFile.CurrentTime;
                next = dtmfAudio.Skip();
                var duration = waveFile.CurrentTime - start;

                yield return new DtmfPosition(current, start, duration);
            }
        }
    }
}
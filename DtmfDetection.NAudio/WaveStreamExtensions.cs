namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public static class WaveStreamExtensions
    {
        public static IEnumerable<DtmfOccurence> DtmfTones(this WaveStream waveFile)
        {
            var dtmfAudio = new DtmfAudio(new StaticSampleSource(waveFile));
            var next = DtmfTone.None;

            while (next != DtmfTone.None || dtmfAudio.Wait() != DtmfTone.None)
            {
                var current = dtmfAudio.CurrentDtmfTone;
                var start = waveFile.CurrentTime;
                next = dtmfAudio.Skip();
                var duration = waveFile.CurrentTime - start;

                yield return new DtmfOccurence(current, start, duration);
            }
        }
    }
}
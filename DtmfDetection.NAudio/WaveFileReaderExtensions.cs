using System.Collections.Generic;

using NAudio.Wave;

namespace DtmfDetection.NAudio
{
    public static class WaveFileReaderExtensions
    {
        public static IEnumerable<DtmfOccurence> FindDtmfTones(this WaveFileReader waveFile)
        {
            var dtmfAudio = new DtmfAudio(new StaticSampleSource(waveFile));

            while (dtmfAudio.DataAvailable)
            {
                dtmfAudio.WaitForDtmfTone();
                var start = waveFile.CurrentTime;

                dtmfAudio.WaitForEndOfDtmfTone();
                var duration = waveFile.CurrentTime - start;

                if (dtmfAudio.LastDtmfTone != DtmfTone.None)
                    yield return new DtmfOccurence(dtmfAudio.LastDtmfTone, start, duration);
            }
        }
    }
}
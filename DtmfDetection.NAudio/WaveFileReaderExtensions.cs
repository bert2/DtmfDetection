namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public static class WaveFileReaderExtensions
    {
        public static IEnumerable<DtmfOccurence> DtmfTones(this WaveFileReader waveFile)
        {
            var dtmfAudio = new DtmfAudio(new StaticSampleSource(waveFile));

            while (dtmfAudio.WaitForDtmfTone() != DtmfTone.None)
            {
                var start = waveFile.CurrentTime;
                dtmfAudio.WaitForEndOfLastDtmfTone();
                var duration = waveFile.CurrentTime - start;

                yield return new DtmfOccurence(dtmfAudio.LastDtmfTone, start, duration);
            }
        }
    }
}
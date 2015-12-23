// TODO No class; should be extension method to WaveFileReader
namespace DtmfDetection.NAudio
{
    using System;
    using System.Collections.Generic;

    using global::NAudio.Wave;

    public class WaveFile : IDisposable
    {
        private readonly DtmfAudio dtmfAudio;

        private readonly WaveFileReader waveFile;

        public WaveFile(string filename)
        {
            waveFile = new WaveFileReader(filename);
            dtmfAudio = new DtmfAudio(waveFile);
        }

        public IEnumerable<DtmfOccurence> DtmfTones
        {
            get
            {
                while (dtmfAudio.SkipToDtmfTone())
                {
                    var start = waveFile.CurrentTime;

                    dtmfAudio.SkipToEndOfDtmfTone();

                    var duration = waveFile.CurrentTime - start;

                    yield return new DtmfOccurence(dtmfAudio.LastDtmfTone, start, duration);
                }
            }
        }

        public void Dispose()
        {
            waveFile.Dispose();
        }
    }
}
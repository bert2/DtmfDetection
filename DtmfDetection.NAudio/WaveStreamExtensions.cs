namespace DtmfDetection.NAudio
{
    using System.Collections.Generic;
    using System.Linq;

    using global::NAudio.Wave;

    /// <summary>Provides an extension method to NAudio's WaveStream class that detects DTMF tones in
    /// audio files or streams.</summary>
    public static class WaveStreamExtensions
    {
        /// <summary>Reads a WaveStream and enumerates all present DTMF tones.</summary>
        /// <remarks>By default this method forces a mono conversion by averaging all audio channels first. Turn it off with the
        ///  forceMono flag in order to analyze each channel separately.</remarks>
        /// <param name="waveFile">The audio data to analyze.</param>
        /// <param name="forceMono">Indicates whether the audio data should be converted to mono first. Default is true.</param>
        /// <returns>All detected DTMF tones along with their positions (i.e. audio channel, start time, and duration).</returns>
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
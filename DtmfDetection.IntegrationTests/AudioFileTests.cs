namespace DtmfDetection.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NAudio;

    using global::NAudio.Wave;
    using global::NAudio.Vorbis;
    
    [TestClass]
    public class AudioFileTests
    {
        [TestMethod]
        public void DetectsLongDtmfTonesCorrectly()
        {
            #region Expected tones

            var expectedTones = new DtmfOccurence[]
                {
                    new DtmfOccurence(DtmfTone.One,   TimeSpan.FromSeconds(02.84), TimeSpan.FromSeconds(2.84)),
                    new DtmfOccurence(DtmfTone.Two,   TimeSpan.FromSeconds(06.80), TimeSpan.FromSeconds(0.16)),
                    new DtmfOccurence(DtmfTone.Three, TimeSpan.FromSeconds(07.40), TimeSpan.FromSeconds(0.16)),
                    new DtmfOccurence(DtmfTone.Four,  TimeSpan.FromSeconds(08.24), TimeSpan.FromSeconds(2.44)),
                    new DtmfOccurence(DtmfTone.Five,  TimeSpan.FromSeconds(12.12), TimeSpan.FromSeconds(0.28)),
                    new DtmfOccurence(DtmfTone.Six,   TimeSpan.FromSeconds(12.80), TimeSpan.FromSeconds(0.12)),
                    new DtmfOccurence(DtmfTone.Seven, TimeSpan.FromSeconds(14.60), TimeSpan.FromSeconds(2.36)),
                    new DtmfOccurence(DtmfTone.Eight, TimeSpan.FromSeconds(17.72), TimeSpan.FromSeconds(0.16)),
                    new DtmfOccurence(DtmfTone.Nine,  TimeSpan.FromSeconds(18.48), TimeSpan.FromSeconds(0.16)),
                    new DtmfOccurence(DtmfTone.Hash,  TimeSpan.FromSeconds(19.20), TimeSpan.FromSeconds(0.28)),
                    new DtmfOccurence(DtmfTone.Zero,  TimeSpan.FromSeconds(19.92), TimeSpan.FromSeconds(0.08)),
                    new DtmfOccurence(DtmfTone.Star,  TimeSpan.FromSeconds(20.56), TimeSpan.FromSeconds(0.16)),
                    new DtmfOccurence(DtmfTone.One,   TimeSpan.FromSeconds(22.12), TimeSpan.FromSeconds(1.76))
                };

            #endregion

            using (var waveFile = new WaveFileReader("TestData/long_dtmf_tones.wav"))
            {
                var actualTones = waveFile.DtmfTones().ToArray();

                AssertEqual(expectedTones, actualTones);
            }
        }

        /// <summary>Test data has been copied from https://en.wikipedia.org/wiki/File:DTMF_dialing.ogg (no license, public domain).</summary>
        [TestMethod]
        public void DetectsVeryShortDtmfTonesCorrectly()
        {
            #region Expected tones

            var expectedTones = new DtmfTone[]
                {
                    DtmfTone.Zero, DtmfTone.Six, DtmfTone.Nine, DtmfTone.Six, DtmfTone.Six, DtmfTone.Seven, DtmfTone.Five, DtmfTone.Three, DtmfTone.Five, DtmfTone.Six,

                    DtmfTone.Four, DtmfTone.Six, DtmfTone.Four, DtmfTone.Six, DtmfTone.Four, DtmfTone.One, DtmfTone.Five, DtmfTone.One, DtmfTone.Eight, DtmfTone.Zero,

                    DtmfTone.Two, DtmfTone.Three, DtmfTone.Three, DtmfTone.Six, DtmfTone.Seven, DtmfTone.Three, DtmfTone.One, DtmfTone.Four, DtmfTone.One, DtmfTone.Six,

                    DtmfTone.Three, DtmfTone.Six, DtmfTone.Zero, DtmfTone.Eight, DtmfTone.Three, DtmfTone.Three, DtmfTone.Eight, DtmfTone.One, DtmfTone.Six, DtmfTone.Zero,

                    DtmfTone.Four, DtmfTone.Four, DtmfTone.Zero, DtmfTone.Zero, DtmfTone.Eight, DtmfTone.Two, DtmfTone.Six, DtmfTone.One, DtmfTone.Four, DtmfTone.Six,

                    DtmfTone.Six, DtmfTone.Two, DtmfTone.Five, DtmfTone.Three, DtmfTone.Six, DtmfTone.Eight, DtmfTone.Nine, DtmfTone.Six, DtmfTone.Three, DtmfTone.Eight,

                    DtmfTone.Eight, DtmfTone.Four, DtmfTone.Eight, DtmfTone.Two, DtmfTone.One, DtmfTone.Three, DtmfTone.Eight, DtmfTone.One, DtmfTone.Seven, DtmfTone.Eight,

                    DtmfTone.Five, DtmfTone.Zero, DtmfTone.Seven, DtmfTone.Three, DtmfTone.Six, DtmfTone.Four, DtmfTone.Three, DtmfTone.Three, DtmfTone.Nine, DtmfTone.Nine
                };

            #endregion

            using (var waveFile = new VorbisWaveReader("TestData/very_short_dtmf_tones.ogg"))
            {
                var actualTones = waveFile.DtmfTones().Select(t => t.DtmfTone).ToArray();

                AssertEqual(expectedTones, actualTones);
            }
        }

        private static void AssertEqual<T>(IList<T> expecteds, IList<T> actuals)
        {
            for (var i = 0; i < Math.Min(expecteds.Count, actuals.Count); i++)
                Assert.AreEqual(expecteds[i], actuals[i], $"Items at index {i} do not match");

            Assert.AreEqual(expecteds.Count, actuals.Count, "The lists are not of the same length.");
        }
    }
}

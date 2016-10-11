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

            var expectedTones = new[]
            {
                new DtmfOccurence(DtmfTone.One, 0, TimeSpan.FromSeconds(2.819), TimeSpan.FromSeconds(2.819)),
                new DtmfOccurence(DtmfTone.Two, 0, TimeSpan.FromSeconds(6.766), TimeSpan.FromSeconds(0.153)),
                new DtmfOccurence(DtmfTone.Three, 0, TimeSpan.FromSeconds(7.329), TimeSpan.FromSeconds(0.180)),
                new DtmfOccurence(DtmfTone.Four, 0, TimeSpan.FromSeconds(8.252), TimeSpan.FromSeconds(2.357)),
                new DtmfOccurence(DtmfTone.Five, 0, TimeSpan.FromSeconds(12.070), TimeSpan.FromSeconds(0.026)),
                new DtmfOccurence(DtmfTone.Five, 0, TimeSpan.FromSeconds(12.121), TimeSpan.FromSeconds(0.205)),
                new DtmfOccurence(DtmfTone.Six, 0, TimeSpan.FromSeconds(12.762), TimeSpan.FromSeconds(0.102)),
                new DtmfOccurence(DtmfTone.Seven, 0, TimeSpan.FromSeconds(14.556), TimeSpan.FromSeconds(0.025)),
                new DtmfOccurence(DtmfTone.Seven, 0, TimeSpan.FromSeconds(14.607), TimeSpan.FromSeconds(2.306)),
                new DtmfOccurence(DtmfTone.Eight, 0, TimeSpan.FromSeconds(17.733), TimeSpan.FromSeconds(0.103)),
                new DtmfOccurence(DtmfTone.Nine, 0, TimeSpan.FromSeconds(18.476), TimeSpan.FromSeconds(0.103)),
                new DtmfOccurence(DtmfTone.Hash, 0, TimeSpan.FromSeconds(19.168), TimeSpan.FromSeconds(0.256)),
                new DtmfOccurence(DtmfTone.Zero, 0, TimeSpan.FromSeconds(19.886), TimeSpan.FromSeconds(0.051)),
                new DtmfOccurence(DtmfTone.Star, 0, TimeSpan.FromSeconds(20.501), TimeSpan.FromSeconds(0.153)),
                new DtmfOccurence(DtmfTone.One, 0, TimeSpan.FromSeconds(22.064), TimeSpan.FromSeconds(1.768))
            };

            #endregion

            using (var waveFile = new Mp3FileReader("TestData/long_dtmf_tones.mp3"))
            {
                var actualTones = waveFile.DtmfTones().ToArray();

                AssertEqual(expectedTones, actualTones);
            }
        }

        [TestMethod]
        public void DetectsDtmfTonesInStereoAudioCorrectly()
        {
            #region Expected tones

            var expectedTones = new[]
            {
                new DtmfOccurence(DtmfTone.One, 0, TimeSpan.FromSeconds(2.819), TimeSpan.FromSeconds(2.819)),
                new DtmfOccurence(DtmfTone.One, 1, TimeSpan.FromSeconds(2.819), TimeSpan.FromSeconds(2.819)),
                new DtmfOccurence(DtmfTone.Two, 0, TimeSpan.FromSeconds(6.766), TimeSpan.FromSeconds(0.153)),
                new DtmfOccurence(DtmfTone.Two, 1, TimeSpan.FromSeconds(6.766), TimeSpan.FromSeconds(0.153)),
                new DtmfOccurence(DtmfTone.Three, 0, TimeSpan.FromSeconds(7.329), TimeSpan.FromSeconds(0.180)),
                new DtmfOccurence(DtmfTone.Three, 1, TimeSpan.FromSeconds(7.329), TimeSpan.FromSeconds(0.180)),
                new DtmfOccurence(DtmfTone.Four, 0, TimeSpan.FromSeconds(8.252), TimeSpan.FromSeconds(2.357)),
                new DtmfOccurence(DtmfTone.Four, 1, TimeSpan.FromSeconds(8.252), TimeSpan.FromSeconds(2.357)),
                new DtmfOccurence(DtmfTone.Five, 0, TimeSpan.FromSeconds(12.070), TimeSpan.FromSeconds(0.026)),
                new DtmfOccurence(DtmfTone.Five, 1, TimeSpan.FromSeconds(12.070), TimeSpan.FromSeconds(0.026)),
                new DtmfOccurence(DtmfTone.Five, 0, TimeSpan.FromSeconds(12.121), TimeSpan.FromSeconds(0.205)),
                new DtmfOccurence(DtmfTone.Five, 1, TimeSpan.FromSeconds(12.121), TimeSpan.FromSeconds(0.205)),
                new DtmfOccurence(DtmfTone.Six, 0, TimeSpan.FromSeconds(12.762), TimeSpan.FromSeconds(0.102)),
                new DtmfOccurence(DtmfTone.Six, 1, TimeSpan.FromSeconds(12.762), TimeSpan.FromSeconds(0.102)),
                new DtmfOccurence(DtmfTone.Seven, 0, TimeSpan.FromSeconds(14.556), TimeSpan.FromSeconds(0.025)),
                new DtmfOccurence(DtmfTone.Seven, 1, TimeSpan.FromSeconds(14.556), TimeSpan.FromSeconds(0.025)),
                new DtmfOccurence(DtmfTone.Seven, 0, TimeSpan.FromSeconds(14.607), TimeSpan.FromSeconds(2.306)),
                new DtmfOccurence(DtmfTone.Seven, 1, TimeSpan.FromSeconds(14.607), TimeSpan.FromSeconds(2.306)),
                new DtmfOccurence(DtmfTone.Eight, 0, TimeSpan.FromSeconds(17.733), TimeSpan.FromSeconds(0.103)),
                new DtmfOccurence(DtmfTone.Eight, 1, TimeSpan.FromSeconds(17.733), TimeSpan.FromSeconds(0.103)),
                new DtmfOccurence(DtmfTone.Nine, 0, TimeSpan.FromSeconds(18.476), TimeSpan.FromSeconds(0.103)),
                new DtmfOccurence(DtmfTone.Nine, 1, TimeSpan.FromSeconds(18.476), TimeSpan.FromSeconds(0.103)),
                new DtmfOccurence(DtmfTone.Hash, 0, TimeSpan.FromSeconds(19.168), TimeSpan.FromSeconds(0.256)),
                new DtmfOccurence(DtmfTone.Hash, 1, TimeSpan.FromSeconds(19.168), TimeSpan.FromSeconds(0.256)),
                new DtmfOccurence(DtmfTone.Zero, 0, TimeSpan.FromSeconds(19.886), TimeSpan.FromSeconds(0.051)),
                new DtmfOccurence(DtmfTone.Zero, 1, TimeSpan.FromSeconds(19.886), TimeSpan.FromSeconds(0.051)),
                new DtmfOccurence(DtmfTone.Star, 0, TimeSpan.FromSeconds(20.501), TimeSpan.FromSeconds(0.153)),
                new DtmfOccurence(DtmfTone.Star, 1, TimeSpan.FromSeconds(20.501), TimeSpan.FromSeconds(0.153)),
                new DtmfOccurence(DtmfTone.One, 0, TimeSpan.FromSeconds(22.064), TimeSpan.FromSeconds(1.768)),
                new DtmfOccurence(DtmfTone.One, 1, TimeSpan.FromSeconds(22.064), TimeSpan.FromSeconds(1.768))
            };

            #endregion

            using (var waveFile = new Mp3FileReader("TestData/long_dtmf_tones.mp3"))
            {
                var actualTones = waveFile.DtmfTones(false).ToArray();

                AssertEqual(expectedTones, actualTones);
            }
        }

        [TestMethod]
        public void DetectsStereoDtmfTonesCorrectly()
        {
            #region Expected tones

            var expectedTones = new[]
            {
                new DtmfOccurence(DtmfTone.One, 0, TimeSpan.FromSeconds(0.026), TimeSpan.FromSeconds(0.999)),
                new DtmfOccurence(DtmfTone.Two, 1, TimeSpan.FromSeconds(2.024), TimeSpan.FromSeconds(1.0)),
                new DtmfOccurence(DtmfTone.Three, 0, TimeSpan.FromSeconds(4.023), TimeSpan.FromSeconds(1.999)),
                new DtmfOccurence(DtmfTone.Four, 1, TimeSpan.FromSeconds(5.023), TimeSpan.FromSeconds(1.998)),
                new DtmfOccurence(DtmfTone.Five, 0, TimeSpan.FromSeconds(8.021), TimeSpan.FromSeconds(0.999)),
                new DtmfOccurence(DtmfTone.Six, 1, TimeSpan.FromSeconds(8.021), TimeSpan.FromSeconds(0.999)),
                new DtmfOccurence(DtmfTone.Eight, 1, TimeSpan.FromSeconds(11.019), TimeSpan.FromSeconds(0.999)),
                new DtmfOccurence(DtmfTone.Seven, 0, TimeSpan.FromSeconds(10.019), TimeSpan.FromSeconds(2.999)),
                new DtmfOccurence(DtmfTone.Nine, 0, TimeSpan.FromSeconds(14.017), TimeSpan.FromSeconds(0.999)),
                new DtmfOccurence(DtmfTone.Zero, 0, TimeSpan.FromSeconds(15.016), TimeSpan.FromSeconds(0.984))
            };

            #endregion

            using (var waveFile = new WaveFileReader("TestData/stereo_dtmf_tones.wav"))
            {
                var actualTones = waveFile.DtmfTones(false).ToArray();

                AssertEqual(expectedTones, actualTones);
            }
        }

        [TestMethod]
        public void DetectsAllStereoDtmfTonesWhenMonoIsForced()
        {
            #region Expected tones

            var expectedTones = new[]
            {
                new DtmfOccurence(DtmfTone.One, 0, TimeSpan.FromSeconds(0.026), TimeSpan.FromSeconds(0.999)),
                new DtmfOccurence(DtmfTone.Two, 0, TimeSpan.FromSeconds(2.024), TimeSpan.FromSeconds(1.0)),
                new DtmfOccurence(DtmfTone.Three, 0, TimeSpan.FromSeconds(4.023), TimeSpan.FromSeconds(0.977))
            };

            #endregion

            using (var waveFile = new WaveFileReader("TestData/stereo_dtmf_tones_no_overlap.wav"))
            {
                var actualTones = waveFile.DtmfTones(forceMono: true).ToArray();

                AssertEqual(expectedTones, actualTones);
            }
        }

        /// <summary>Test data has been taken from https://en.wikipedia.org/wiki/File:DTMF_dialing.ogg (no license, public domain). 
        /// Mark/space of the DTMF sequences is about 60/40.</summary>
        [TestMethod]
        public void DetectsVeryShortDtmfTonesCorrectly()
        {
            #region Expected tones

            var expectedTones = new[]
            {
                DtmfTone.Zero,
                DtmfTone.Six,
                DtmfTone.Nine,
                DtmfTone.Six,
                DtmfTone.Six,
                DtmfTone.Seven,
                DtmfTone.Five,
                DtmfTone.Three,
                DtmfTone.Five,
                DtmfTone.Six,

                DtmfTone.Four,
                DtmfTone.Six,
                DtmfTone.Four,
                DtmfTone.Six,
                DtmfTone.Four,
                DtmfTone.One,
                DtmfTone.Five,
                DtmfTone.One,
                DtmfTone.Eight,
                DtmfTone.Zero,

                DtmfTone.Two,
                DtmfTone.Three,
                DtmfTone.Three,
                DtmfTone.Six,
                DtmfTone.Seven,
                DtmfTone.Three,
                DtmfTone.One,
                DtmfTone.Four,
                DtmfTone.One,
                DtmfTone.Six,

                DtmfTone.Three,
                DtmfTone.Six,
                DtmfTone.Zero,
                DtmfTone.Eight,
                DtmfTone.Three,
                DtmfTone.Three,
                DtmfTone.Eight,
                DtmfTone.One,
                DtmfTone.Six,
                DtmfTone.Zero,

                DtmfTone.Four,
                DtmfTone.Four,
                DtmfTone.Zero,
                DtmfTone.Zero,
                DtmfTone.Eight,
                DtmfTone.Two,
                DtmfTone.Six,
                DtmfTone.One,
                DtmfTone.Four,
                DtmfTone.Six,

                DtmfTone.Six,
                DtmfTone.Two,
                DtmfTone.Five,
                DtmfTone.Three,
                DtmfTone.Six,
                DtmfTone.Eight,
                DtmfTone.Nine,
                DtmfTone.Six,
                DtmfTone.Three,
                DtmfTone.Eight,

                DtmfTone.Eight,
                DtmfTone.Four,
                DtmfTone.Eight,
                DtmfTone.Two,
                DtmfTone.One,
                DtmfTone.Three,
                DtmfTone.Eight,
                DtmfTone.One,
                DtmfTone.Seven,
                DtmfTone.Eight,

                DtmfTone.Five,
                DtmfTone.Zero,
                DtmfTone.Seven,
                DtmfTone.Three,
                DtmfTone.Six,
                DtmfTone.Four,
                DtmfTone.Three,
                DtmfTone.Three,
                DtmfTone.Nine,
                DtmfTone.Nine
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
            Assert.AreEqual(
                expecteds.Count,
                actuals.Count,
                "The lists are not of the same length.\nFirst mismatch: " + GetFirstMismatch(expecteds, actuals));

            AssertEqualElements(expecteds, actuals);
        }

        private static void AssertEqualElements<T>(IList<T> expecteds, IList<T> actuals)
        {
            for (var i = 0; i < Math.Min(expecteds.Count, actuals.Count); i++)
                Assert.AreEqual(expecteds[i], actuals[i], $"Items at index {i} do not match");
        }

        private static string GetFirstMismatch<T>(IList<T> expecteds, IList<T> actuals)
        {
            try
            {
                AssertEqualElements(expecteds, actuals);
            }
            catch (AssertFailedException e)
            {
                return e.Message;
            }

            return string.Empty;
        }
    }
}

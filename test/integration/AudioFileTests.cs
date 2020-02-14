namespace Integration {
    using System;
    using System.Linq;
    using DtmfDetection;
    using DtmfDetection.NAudio;
    using NAudio.Vorbis;
    using NAudio.Wave;
    using Shouldly;
    using Xunit;

    public class AudioFileTests {
        [Fact]
        public void LongDtmfTones() {
            using var file = new AudioFileReader("./testdata/long_dtmf_tones.mp3");
            var dtmfs = file.DtmfChanges();
            dtmfs.ShouldBe(new[] {
                DtmfChange.Start(PhoneKey.One,   TimeSpan.Parse("00:00:02.7675736"), 0),
                DtmfChange.Stop(PhoneKey.One,    TimeSpan.Parse("00:00:05.5607029"), 0),
                DtmfChange.Start(PhoneKey.Two,   TimeSpan.Parse("00:00:06.7138321"), 0),
                DtmfChange.Stop(PhoneKey.Two,    TimeSpan.Parse("00:00:06.8675736"), 0),
                DtmfChange.Start(PhoneKey.Three, TimeSpan.Parse("00:00:07.3031972"), 0),
                DtmfChange.Stop(PhoneKey.Three,  TimeSpan.Parse("00:00:07.4313378"), 0),
                DtmfChange.Start(PhoneKey.Four,  TimeSpan.Parse("00:00:08.2000680"), 0),
                DtmfChange.Stop(PhoneKey.Four,   TimeSpan.Parse("00:00:10.5319501"), 0),
                DtmfChange.Start(PhoneKey.Five,  TimeSpan.Parse("00:00:12.0950793"), 0),
                DtmfChange.Stop(PhoneKey.Five,   TimeSpan.Parse("00:00:12.2744444"), 0),
                DtmfChange.Start(PhoneKey.Six,   TimeSpan.Parse("00:00:12.7357142"), 0),
                DtmfChange.Stop(PhoneKey.Six,    TimeSpan.Parse("00:00:12.8125850"), 0),
                DtmfChange.Start(PhoneKey.Seven, TimeSpan.Parse("00:00:14.5038321"), 0),
                DtmfChange.Stop(PhoneKey.Seven,  TimeSpan.Parse("00:00:14.5294557"), 0),
                DtmfChange.Start(PhoneKey.Seven, TimeSpan.Parse("00:00:14.5550793"), 0),
                DtmfChange.Stop(PhoneKey.Seven,  TimeSpan.Parse("00:00:16.8357142"), 0),
                DtmfChange.Start(PhoneKey.Eight, TimeSpan.Parse("00:00:17.6813378"), 0),
                DtmfChange.Stop(PhoneKey.Eight,  TimeSpan.Parse("00:00:17.7582086"), 0),
                DtmfChange.Start(PhoneKey.Nine,  TimeSpan.Parse("00:00:18.4500680"), 0),
                DtmfChange.Stop(PhoneKey.Nine,   TimeSpan.Parse("00:00:18.5269614"), 0),
                DtmfChange.Start(PhoneKey.Hash,  TimeSpan.Parse("00:00:19.1163265"), 0),
                DtmfChange.Stop(PhoneKey.Hash,   TimeSpan.Parse("00:00:19.1419501"), 0),
                DtmfChange.Start(PhoneKey.Hash,  TimeSpan.Parse("00:00:19.1675736"), 0),
                DtmfChange.Stop(PhoneKey.Hash,   TimeSpan.Parse("00:00:19.3469614"), 0),
                DtmfChange.Start(PhoneKey.Zero,  TimeSpan.Parse("00:00:19.8338321"), 0),
                DtmfChange.Stop(PhoneKey.Zero,   TimeSpan.Parse("00:00:19.8850793"), 0),
                DtmfChange.Start(PhoneKey.Star,  TimeSpan.Parse("00:00:20.4744444"), 0),
                DtmfChange.Stop(PhoneKey.Star,   TimeSpan.Parse("00:00:20.6025850"), 0),
                DtmfChange.Start(PhoneKey.One,   TimeSpan.Parse("00:00:22.0119501"), 0),
                DtmfChange.Stop(PhoneKey.One,    TimeSpan.Parse("00:00:23.7544444"), 0)
            });
        }

        [Fact]
        public void StereoDtmfTones() {
            using var file = new AudioFileReader("./testdata/stereo_dtmf_tones.wav");
            var dtmfs = file.DtmfChanges(forceMono: false);
            dtmfs.ShouldBe(new[] {
                DtmfChange.Start(PhoneKey.One,   TimeSpan.Parse("00:00:00"), 0),
                DtmfChange.Stop(PhoneKey.One,    TimeSpan.Parse("00:00:00.9994557"), 0),
                DtmfChange.Start(PhoneKey.Two,   TimeSpan.Parse("00:00:01.9988208"), 1),
                DtmfChange.Stop(PhoneKey.Two,    TimeSpan.Parse("00:00:02.9982086"), 1),
                DtmfChange.Start(PhoneKey.Three, TimeSpan.Parse("00:00:03.9975736"), 0),
                DtmfChange.Start(PhoneKey.Four,  TimeSpan.Parse("00:00:04.9969614"), 1),
                DtmfChange.Stop(PhoneKey.Three,  TimeSpan.Parse("00:00:05.9963265"), 0),
                DtmfChange.Stop(PhoneKey.Four,   TimeSpan.Parse("00:00:06.9957142"), 1),
                DtmfChange.Start(PhoneKey.Five,  TimeSpan.Parse("00:00:07.9950793"), 0),
                DtmfChange.Start(PhoneKey.Six,   TimeSpan.Parse("00:00:07.9950793"), 1),
                DtmfChange.Stop(PhoneKey.Five,   TimeSpan.Parse("00:00:08.9944444"), 0),
                DtmfChange.Stop(PhoneKey.Six,    TimeSpan.Parse("00:00:08.9944444"), 1),
                DtmfChange.Start(PhoneKey.Seven, TimeSpan.Parse("00:00:09.9938321"), 0),
                DtmfChange.Start(PhoneKey.Eight, TimeSpan.Parse("00:00:11.0188208"), 1),
                DtmfChange.Stop(PhoneKey.Eight,  TimeSpan.Parse("00:00:11.9925850"), 1),
                DtmfChange.Stop(PhoneKey.Seven,  TimeSpan.Parse("00:00:12.9919501"), 0),
                DtmfChange.Start(PhoneKey.Nine,  TimeSpan.Parse("00:00:14.0169614"), 0),
                DtmfChange.Stop(PhoneKey.Nine,   TimeSpan.Parse("00:00:14.9907029"), 0),
                DtmfChange.Start(PhoneKey.Zero,  TimeSpan.Parse("00:00:15.0163265"), 0),
                DtmfChange.Stop(PhoneKey.Zero,   TimeSpan.Parse("00:00:15.9900680"), 0),
            });
        }

        [Fact]
        public void VeryShortDtmfTones() {
            using var file = new VorbisWaveReader("./testdata/very_short_dtmf_tones.ogg");
            var dtmfs = file.DtmfChanges();
            dtmfs.Where(d => d.IsStart).Select(d => d.Key).ShouldBe(new[]
            {
                PhoneKey.Zero,
                PhoneKey.Six,
                PhoneKey.Nine,
                PhoneKey.Six,
                PhoneKey.Six,
                PhoneKey.Seven,
                PhoneKey.Five,
                PhoneKey.Three,
                PhoneKey.Five,
                PhoneKey.Six,

                PhoneKey.Four,
                PhoneKey.Six,
                PhoneKey.Four,
                PhoneKey.Six,
                PhoneKey.Four,
                PhoneKey.One,
                PhoneKey.Five,
                PhoneKey.One,
                PhoneKey.Eight,
                PhoneKey.Zero,

                PhoneKey.Two,
                PhoneKey.Three,
                PhoneKey.Three,
                PhoneKey.Six,
                PhoneKey.Seven,
                PhoneKey.Three,
                PhoneKey.One,
                PhoneKey.Four,
                PhoneKey.One,
                PhoneKey.Six,

                PhoneKey.Three,
                PhoneKey.Six,
                PhoneKey.Zero,
                PhoneKey.Eight,
                PhoneKey.Three,
                PhoneKey.Three,
                PhoneKey.Eight,
                PhoneKey.One,
                PhoneKey.Six,
                PhoneKey.Zero,

                PhoneKey.Four,
                PhoneKey.Four,
                PhoneKey.Zero,
                PhoneKey.Zero,
                PhoneKey.Eight,
                PhoneKey.Two,
                PhoneKey.Six,
                PhoneKey.One,
                PhoneKey.Four,
                PhoneKey.Six,

                PhoneKey.Six,
                PhoneKey.Two,
                PhoneKey.Five,
                PhoneKey.Three,
                PhoneKey.Six,
                PhoneKey.Eight,
                PhoneKey.Nine,
                PhoneKey.Six,
                PhoneKey.Three,
                PhoneKey.Eight,

                PhoneKey.Eight,
                PhoneKey.Four,
                PhoneKey.Eight,
                PhoneKey.Two,
                PhoneKey.One,
                PhoneKey.Three,
                PhoneKey.Eight,
                PhoneKey.One,
                PhoneKey.Seven,
                PhoneKey.Eight,

                PhoneKey.Five,
                PhoneKey.Zero,
                PhoneKey.Seven,
                PhoneKey.Three,
                PhoneKey.Six,
                PhoneKey.Four,
                PhoneKey.Three,
                PhoneKey.Three,
                PhoneKey.Nine,
                PhoneKey.Nine
            });
        }
    }
}

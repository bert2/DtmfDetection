namespace Integration {
    using DtmfDetection.NAudio;
    using NAudio.Wave;
    using Xunit;

    public class AudioFileTests {
        [Fact]
        public void Test1() {
            using var file = new AudioFileReader("./testdata/long_dtmf_tones.mp3");
            var dtmfs = file.DtmfChanges(true);
        }
    }
}

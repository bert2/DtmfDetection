namespace Unit {
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;
    using Shouldly;
    using Xunit;

    using static TestToneGenerator;

    public class SampleProcessorTests {
        [Fact]
        public void Test() {
            var s = DtmfTone(PhoneKey.Five).Take(500).AsSamples();
            var p = new SampleProcessor(s);

            var dtmfs = new List<DtmfChange>();
            while (p.CanRead) dtmfs.AddRange(p.ProcessNext());

            dtmfs.ShouldBe(new[] { DtmfChange.Start(PhoneKey.Five, 0), DtmfChange.Stop(PhoneKey.Five, 0) });
        }

        [Fact]
        public void Test2() {
            var space = Constant(.0f).Take(160);
            var s = space
                .Concat(DtmfTone(PhoneKey.A).Take(320))
                .Concat(space)
                .Concat(DtmfTone(PhoneKey.C).Take(320))
                .Concat(space)
                .Concat(DtmfTone(PhoneKey.A).Take(320))
                .Concat(space)
                .Concat(DtmfTone(PhoneKey.B).Take(320))
                .Concat(space)
                .AsSamples();
            var p = new SampleProcessor(s);

            var dtmfs = new List<DtmfChange>();
            while (p.CanRead) dtmfs.AddRange(p.ProcessNext());

            dtmfs.ShouldBe(new[] {
                DtmfChange.Start(PhoneKey.A, 0), DtmfChange.Stop(PhoneKey.A, 0),
                DtmfChange.Start(PhoneKey.C, 0), DtmfChange.Stop(PhoneKey.C, 0),
                DtmfChange.Start(PhoneKey.A, 0), DtmfChange.Stop(PhoneKey.A, 0),
                DtmfChange.Start(PhoneKey.B, 0), DtmfChange.Stop(PhoneKey.B, 0)
            });
        }
    }
}

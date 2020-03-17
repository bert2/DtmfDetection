namespace Integration {
    using System;
    using System.Linq;
    using DtmfDetection;
    using Shouldly;
    using Xunit;

    using static DtmfDetection.DtmfGenerator;

    public class FloatArrayTests {
        [Fact]
        public void PcmData() =>
            Concat(Space(), Mark(PhoneKey.A), Space(), Mark(PhoneKey.C), Space(), Mark(PhoneKey.A), Space(), Mark(PhoneKey.B), Space()).ToArray()
            .DtmfChanges()
            .ShouldBe(new[] {
                DtmfChange.Start(PhoneKey.A, TimeSpan.Parse("00:00:00.0000026"), 0),
                DtmfChange.Stop(PhoneKey.A,  TimeSpan.Parse("00:00:00.0000051"), 0),
                DtmfChange.Start(PhoneKey.C, TimeSpan.Parse("00:00:00.0000077"), 0),
                DtmfChange.Stop(PhoneKey.C,  TimeSpan.Parse("00:00:00.0000128"), 0),
                DtmfChange.Start(PhoneKey.A, TimeSpan.Parse("00:00:00.0000154"), 0),
                DtmfChange.Stop(PhoneKey.A,  TimeSpan.Parse("00:00:00.0000179"), 0),
                DtmfChange.Start(PhoneKey.B, TimeSpan.Parse("00:00:00.0000205"), 0),
                DtmfChange.Stop(PhoneKey.B,  TimeSpan.Parse("00:00:00.0000231"), 0)
            });
    }
}

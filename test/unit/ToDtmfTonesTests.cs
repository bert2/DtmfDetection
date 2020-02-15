namespace Unit {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DtmfDetection;
    using Shouldly;
    using Xunit;

    public class ToDtmfTonesTests {
        [Fact]
        public void MergesStartsAndStopsIntoDtmfTones() =>
            new List<DtmfChange> {
                DtmfChange.Start(PhoneKey.A, TimeSpan.FromSeconds(1), 0),
                DtmfChange.Stop(PhoneKey.A, TimeSpan.FromSeconds(3), 0),

                DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(4), 0),
                DtmfChange.Stop(PhoneKey.B, TimeSpan.FromSeconds(7), 0),

                DtmfChange.Start(PhoneKey.C, TimeSpan.FromSeconds(9), 0),
                DtmfChange.Stop(PhoneKey.C, TimeSpan.FromSeconds(10), 0)
            }
            .ToDtmfTones()
            .ShouldBe(new[] {
                new DtmfTone(PhoneKey.A, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 0),
                new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(3), 0),
                new DtmfTone(PhoneKey.C, TimeSpan.FromSeconds(9), TimeSpan.FromSeconds(1), 0)
            });

        [Fact]
        public void CanHandleOverlapInSameChannel() =>
            new List<DtmfChange> {
                DtmfChange.Start(PhoneKey.A, TimeSpan.FromSeconds(1), 0),
                DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(2), 0),
                DtmfChange.Stop(PhoneKey.B, TimeSpan.FromSeconds(3), 0),
                DtmfChange.Start(PhoneKey.C, TimeSpan.FromSeconds(4), 0),
                DtmfChange.Stop(PhoneKey.A, TimeSpan.FromSeconds(5), 0),
                DtmfChange.Stop(PhoneKey.C, TimeSpan.FromSeconds(6), 0)
            }
            .ToDtmfTones()
            .ShouldBe(new[] {
                new DtmfTone(PhoneKey.A, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4), 0),
                new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), 0),
                new DtmfTone(PhoneKey.C, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(2), 0)
            });

        [Fact]
        public void CanHandleOverlapAcrossChannels() =>
            new List<DtmfChange> {
                DtmfChange.Start(PhoneKey.A, TimeSpan.FromSeconds(1), 0),
                DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(2), 1),
                DtmfChange.Stop(PhoneKey.B, TimeSpan.FromSeconds(3), 1),
                DtmfChange.Start(PhoneKey.C, TimeSpan.FromSeconds(4), 2),
                DtmfChange.Stop(PhoneKey.A, TimeSpan.FromSeconds(5), 0),
                DtmfChange.Stop(PhoneKey.C, TimeSpan.FromSeconds(6), 2)
            }
            .ToDtmfTones()
            .ShouldBe(new[] {
                new DtmfTone(PhoneKey.A, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4), 0),
                new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), 1),
                new DtmfTone(PhoneKey.C, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(2), 2)
            });

        [Fact]
        public void ThrowsWhenStopIsMissing() => new Action(() =>
            new List<DtmfChange> {
                DtmfChange.Start(PhoneKey.A, TimeSpan.FromSeconds(1), 0)
            }
            .ToDtmfTones().ToList())
            .ShouldThrow<InvalidOperationException>();
    }
}

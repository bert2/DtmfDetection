namespace Unit {
    using System;
    using System.Collections.Generic;
    using DtmfDetection;
    using Shouldly;
    using Xunit;

    public class DtmfChangeTests {
        [Fact]
        public void ToStringPrintsInfoLine() =>
            DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3)
            .ToString()
            .ShouldBe("B started @ 00:00:05 (ch: 3)");

        #region Equality implementations

        [Fact]
        public void ImplementsIEquatable() =>
            new HashSet<DtmfChange> { DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3) }
            .Contains(DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3))
            .ShouldBeTrue();

        [Fact]
        public void OverridesGetHashCode() =>
            DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3).GetHashCode()
            .ShouldNotBe(DtmfChange.Stop(PhoneKey.C, TimeSpan.FromSeconds(2), 4).GetHashCode());

        [Fact]
        public void OverridesEquals() =>
            DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3)
            .Equals((object)DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3))
            .ShouldBeTrue();

        [Fact]
        public void OverridesEqualsOperator() =>
            (DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3) == DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3))
            .ShouldBeTrue();

        [Fact]
        public void OverridesNotEqualsOperator() =>
            (DtmfChange.Start(PhoneKey.B, TimeSpan.FromSeconds(5), 3) != DtmfChange.Stop(PhoneKey.C, TimeSpan.FromSeconds(2), 4))
            .ShouldBeTrue();

        #endregion Equality implementations
    }
}

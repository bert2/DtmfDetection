namespace Unit {
    using System;
    using System.Collections.Generic;
    using DtmfDetection;
    using Shouldly;
    using Xunit;

    public class DtmfToneTests {
        [Fact]
        public void ToStringPrintsInfoLine() =>
            new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3)
            .ToString()
            .ShouldBe("B @ 00:00:05 (len: 00:00:00.0400000, ch: 3)");

        #region Equality implementations

        [Fact]
        public void ImplementsIEquatable() =>
            new HashSet<DtmfTone> { new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3) }
            .Contains(new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3))
            .ShouldBeTrue();

        [Fact]
        public void OverridesGetHashCode() =>
            new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3).GetHashCode()
            .ShouldNotBe(new DtmfTone(PhoneKey.C, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(30), 4).GetHashCode());

        [Fact]
        public void OverridesEquals() =>
            new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3)
            .Equals((object)new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3))
            .ShouldBeTrue();

        [Fact]
        public void OverridesEqualsOperator() =>
            (new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3)
            == new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3))
            .ShouldBeTrue();

        [Fact]
        public void OverridesNotEqualsOperator() =>
            (new DtmfTone(PhoneKey.B, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(40), 3)
            != new DtmfTone(PhoneKey.C, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(30), 4))
            .ShouldBeTrue();

        #endregion Equality implementations
    }
}

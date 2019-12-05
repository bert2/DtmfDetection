namespace Unit {
    using DtmfDetection;
    using MoreLinq;
    using Shouldly;
    using Xunit;

    using static DtmfDetection.Utils;

    public class PhoneKeyTests {
        [Fact]
        public void ConversionToDtmfToneIsInvertible() => PhoneKeys().ForEach(key =>
            key
            .ToDtmfTone().ToPhoneKey()
            .ShouldBe(key));
    }
}

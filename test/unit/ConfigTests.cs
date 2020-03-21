namespace Unit {
    using System.Collections.Generic;
    using DtmfDetection;
    using Shouldly;
    using Xunit;

    public class ConfigTests {
        #region Equality implementations

        [Fact]
        public void ImplementsIEquatable() =>
            new HashSet<Config> { new Config(1, 2, 3, true) }
            .Contains(new Config(1, 2, 3, true))
            .ShouldBeTrue();

        [Fact]
        public void OverridesGetHashCode() =>
            new Config(1, 2, 3, true).GetHashCode()
            .ShouldNotBe(new Config(4, 5, 6, false).GetHashCode());

        [Fact]
        public void OverridesEquals() =>
            new Config(1, 2, 3, true)
            .Equals((object)new Config(1, 2, 3, true))
            .ShouldBeTrue();

        [Fact]
        public void OverridesEqualsOperator() =>
            (new Config(1, 2, 3, true) == new Config(1, 2, 3, true))
            .ShouldBeTrue();

        [Fact]
        public void OverridesNotEqualsOperator() =>
            (new Config(1, 2, 3, true) != new Config(4, 5, 6, false))
            .ShouldBeTrue();

        #endregion Equality implementations
    }
}

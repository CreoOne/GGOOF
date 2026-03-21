using GGOOF.Version3.ModelFileNames;

namespace GGOOF.UnitTests.Version3.ModelFileNames
{
    public sealed class ModelFileVersionTests
    {
        [Theory]
        [InlineData("v2.5", 2, 5)]
        [InlineData("v4", 4, 0)]
        [InlineData("v09.004", 9, 4)]
        public void FromString_ValidVersionString_ReturnsExpectedVersion(string versionString, uint expectedMajor, uint expectedMinor)
        {
            // Act
            var actual = ModelFileVersion.FromString(versionString);

            // Assert
            var expected = new ModelFileVersion(expectedMajor, expectedMinor);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("2.5")]
        [InlineData("v")]
        [InlineData("v2.")]
        [InlineData("v-5.0")]
        [InlineData("vVI.XI")]
        public void FromString_InvalidVersionString_ThrowsReturnsNull(string versionString)
        {
            // Act
            var actual = ModelFileVersion.FromString(versionString);

            // Assert
            Assert.Null(actual);
        }
    }
}

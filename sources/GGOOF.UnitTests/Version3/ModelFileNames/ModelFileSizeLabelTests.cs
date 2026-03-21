using GGOOF.Version3.ModelFileNames;

namespace GGOOF.UnitTests.Version3.ModelFileNames
{
    public sealed class ModelFileSizeLabelTests
    {
        [Theory]
        [InlineData("1.5B", 0, 1.5, 'B')]
        [InlineData("4x2.5Q", 4, 2.5, 'Q')]
        [InlineData("3x1M", 3, 1, 'M')]
        public void FromString_ValidSizeLabel_ReturnsExpectedModelFileSizeLabel(string sizeLabel, uint expectedExpertCount, decimal expectedCount, char expectedScaleSuffix)
        {
            // Act
            var actual = ModelFileSizeLabel.FromString(sizeLabel);

            // Assert
            var expected = new ModelFileSizeLabel(expectedExpertCount, expectedCount, expectedScaleSuffix);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1.5")]
        [InlineData("4x2.5")]
        [InlineData("B")]
        [InlineData("4xB")]
        [InlineData("2.5X")]
        public void FromString_InvalidSizeLabel_ReturnsNull(string sizeLabel)
        {
            // Act
            var actual = ModelFileSizeLabel.FromString(sizeLabel);

            // Assert
            Assert.Null(actual);
        }
    }
}
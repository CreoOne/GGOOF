using GGOOF.Version3.ModelFileNames;

namespace GGOOF.UnitTests.Version3.ModelFileNames
{
    public sealed class ModelFileSizeAttributeTests
    {
        [Theory]
        [InlineData("ContextLength4K", "ContextLength", 4, 'K')]
        [InlineData("CopyrightStatutoryDamages8M", "CopyrightStatutoryDamages", 8, 'M')]
        public void FromString_ValidSizeAttribute_ReturnsExpectedModelFileSizeLabel(string sizeLabel, string expectedName, decimal expectedCount, char expectedScaleSuffix)
        {
            // Act
            var actual = ModelFileSizeAttribute.FromString(sizeLabel);

            // Assert
            var expected = new ModelFileSizeAttribute(expectedName, expectedCount, expectedScaleSuffix);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1.5B")]
        [InlineData("NoAttentionLeftQQ")]
        [InlineData("AB")]
        public void FromString_InvalidSizeAttribute_ReturnsNull(string sizeLabel)
        {
            // Act
            var actual = ModelFileSizeAttribute.FromString(sizeLabel);

            // Assert
            Assert.Null(actual);
        }
    }
}
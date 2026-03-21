using GGOOF.Version3.ModelFileNames;

namespace GGOOF.UnitTests.Version3.ModelFileNames
{
    public sealed class ModelFileShardTests
    {
        [Theory]
        [InlineData("1-of-4", 1, 4)]
        [InlineData("00002-of-00010", 2, 10)]
        public void FromString_ValidShardString_ReturnsExpectedShard(string shardString, uint expectedIndex, uint expectedTotal)
        {
            // Act
            var actual = ModelFileShard.FromString(shardString);

            // Assert
            var expected = new ModelFileShard(expectedIndex, expectedTotal);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1/4")]
        [InlineData("1-OF-4")]
        public void FromString_InvalidShardString_ReturnsNull(string shardString)
        {
            // Act
            var actual = ModelFileShard.FromString(shardString);

            // Assert
            Assert.Null(actual);
        }
    }
}

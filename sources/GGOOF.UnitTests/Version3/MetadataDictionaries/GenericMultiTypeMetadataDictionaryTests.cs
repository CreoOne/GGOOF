using GGOOF.Version3.MetadataDictionaries;

namespace GGOOF.UnitTests.Version3.MetadataDictionaries
{
    public sealed class GenericMultiTypeMetadataDictionaryTests
    {
        [Fact]
        public void SetAndTryGet_SameType_ReturnsExpectedValue()
        {
            // Arrange
            var metadataDictionary = new GenericMultiTypeMetadataDictionary();
            string key = "TestKey";
            int expectedValue = 42;

            // Act
            metadataDictionary.Set(key, expectedValue);
            bool result = metadataDictionary.TryGet(key, out int actualValue);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void SetAndTryGet_DifferentTypes_ReturnsExpectedValues()
        {
            // Arrange
            var metadataDictionary = new GenericMultiTypeMetadataDictionary();
            string key1 = "IntKey";
            int expectedIntValue = 42;
            string key2 = "StringKey";
            string expectedStringValue = "Hello";

            // Act
            metadataDictionary.Set(key1, expectedIntValue);
            metadataDictionary.Set(key2, expectedStringValue);
            bool actualIntResult = metadataDictionary.TryGet(key1, out int actualIntValue);
            bool actualStringResult = metadataDictionary.TryGet(key2, out string actualStringValue);

            // Assert
            Assert.True(actualIntResult);
            Assert.Equal(expectedIntValue, actualIntValue);
            Assert.True(actualStringResult);
            Assert.Equal(expectedStringValue, actualStringValue);
        }

        [Fact]
        public void TryGet_NonExistentKey_ReturnsFalse()
        {
            // Arrange
            var metadataDictionary = new GenericMultiTypeMetadataDictionary();
            string key = "NonExistentKey";

            // Act
            bool actualResult = metadataDictionary.TryGet(key, out int value);

            // Assert
            Assert.False(actualResult);
            Assert.Equal(default, value);
        }

        [Fact]
        public void Remove_ExistingKey_ReturnsTrue()
        {
            // Arrange
            var metadataDictionary = new GenericMultiTypeMetadataDictionary();
            string key = "TestKey";
            int value = 42;
            metadataDictionary.Set(key, value);

            // Act
            bool actualResult = metadataDictionary.Remove(key);

            // Assert
            Assert.True(actualResult);
            Assert.False(metadataDictionary.TryGet(key, out int nonExistentValue));
            Assert.Equal(default, nonExistentValue);
        }

        [Fact]
        public void Count_ReflectsNumberOfEntries()
        {
            // Arrange
            var metadataDictionary = new GenericMultiTypeMetadataDictionary();
            string key1 = "IntKey";
            int value1 = 42;
            string key2 = "StringKey";
            string value2 = "Hello";

            // Act
            metadataDictionary.Set(key1, value1);
            metadataDictionary.Remove("NonExistentKey");
            metadataDictionary.Set(key2, value2);
            metadataDictionary.Remove(key1);

            // Assert
            Assert.Equal(1UL, metadataDictionary.Count);
        }
    }
}

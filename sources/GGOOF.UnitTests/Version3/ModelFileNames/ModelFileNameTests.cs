using GGOOF.Version3.ModelFileNames;

namespace GGOOF.UnitTests.Version3.ModelFileNames
{
    public sealed class ModelFileNameTests
    {
        [Theory]
        [InlineData("Hermes-2-Pro-Llama-3-8B-F16.gguf", "Hermes 2 Pro Llama 3")]
        [InlineData("Qwen3.5-35B-A3B-UD-Q6_K_XL.gguf", "Qwen3.5")]
        public void FromString_ValidFileName_CorrectModelName(string input, string expected)
        {
            // Act
            var actual = ModelFileName.FromString(input);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Name);
        }

        [Theory]
        [InlineData("not-a-known-arrangement.gguf", "not a known arrangement")]
        public void FromString_InvalidFileName_ReturnsJustName(string input, string expected)
        {
            // Act
            var actual = ModelFileName.FromString(input);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Name);
        }

        [Theory]
        [InlineData("Hermes-2-Pro-Llama-3-8B-F16.gguf", "8B")]
        [InlineData("Qwen3.5-35B-A3B-UD-Q6_K_XL.gguf", "35.B")]
        public void FromString_ValidFileName_CorrectModelSize(string input, string expected)
        {
            // Act
            var actual = ModelFileName.FromString(input);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(ModelFileSizeLabel.FromString(expected), actual.SizeLabel);
        }

        [Theory]
        [InlineData("Qwen3.5-35B-A3B-UD-Q6_K_XL.gguf", "A3B")]
        public void FromString_ValidFileName_CorrectModelSizeAttributes(string input, string expected)
        {
            // Act
            var actual = ModelFileName.FromString(input);

            // Assert
            Assert.NotNull(actual);
            Assert.Contains(ModelFileSizeAttribute.FromString(expected), actual.SizeAttributes);
        }

        [Theory]
        [InlineData("Hermes-2-Pro-Llama-3-8B-v1.0-F16.gguf", "v1.0")]
        [InlineData("Mixtral-8x7B-v0.1-KQ2.gguf", "v0.1")]
        public void FromString_ValidFileName_CorrectModelVersion(string input, string expected)
        {
            // Act
            var actual = ModelFileName.FromString(input);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(ModelFileVersion.FromString(expected), actual.Version);
        }

        [Theory]
        [InlineData("Grok-100B-v1.0-Q4_0-00003-of-00009.gguf", "00003-of-00009")]
        public void FromString_ValidFileName_CorrectModelShard(string input, string expected)
        {
            // Act
            var actual = ModelFileName.FromString(input);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(ModelFileShard.FromString(expected), actual.Shard);
        }

        [Theory]
        [InlineData("Phi-3-mini-3.8B-ContextLength4k-instruct-v1.0.gguf", "instruct")]
        public void FromString_ValidFileName_CorrectFineTune(string input, string expected)
        {
            // Act
            var actual = ModelFileName.FromString(input);
            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.FineTune);
        }

        [Theory]
        [InlineData("Hermes-2-Pro-Llama-3-8B-F16.gguf", "F16")]
        [InlineData("Qwen3.5-35B-A3B-UD-Q6_K_XL.gguf", "Q6_K_XL")]
        [InlineData("Phi-3-mini-3.8B-ContextLength4k-instruct-v1.0.gguf", null)]
        [InlineData("Grok-100B-v1.0-Q4_0-00003-of-00009.gguf", "Q4_0")]
        public void FromString_ValidFileName_CorrectEncoding(string input, string? expected)
        {
            // Act
            var actual = ModelFileName.FromString(input);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Encoding);
        }
    }
}

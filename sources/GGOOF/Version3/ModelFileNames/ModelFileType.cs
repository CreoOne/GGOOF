namespace GGOOF.Version3.ModelFileNames
{
    public enum ModelFileTypeEnum
    {
        LoRA,
        vocab
    }

    public static class ModelFileTypeEnumExtensions
    {
        public static string ToString(this ModelFileTypeEnum type)
            => type switch
            {
                ModelFileTypeEnum.LoRA => nameof(ModelFileTypeEnum.LoRA),
                ModelFileTypeEnum.vocab => nameof(ModelFileTypeEnum.vocab),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

        public static ModelFileTypeEnum? FromString(ReadOnlySpan<char> s)
        {
            if (s.Equals(nameof(ModelFileTypeEnum.LoRA), StringComparison.OrdinalIgnoreCase))
                return ModelFileTypeEnum.LoRA;

            if (s.Equals(nameof(ModelFileTypeEnum.vocab), StringComparison.OrdinalIgnoreCase))
                return ModelFileTypeEnum.vocab;

            return null;
        }
    }
}

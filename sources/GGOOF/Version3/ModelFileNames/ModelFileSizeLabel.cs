using System.Buffers;

namespace GGOOF.Version3.ModelFileNames
{
    public sealed record class ModelFileSizeLabel(uint ExpertCount, decimal Count, char ScaleSuffix)
    {
        private static readonly SearchValues<char> ScaleSuffixesSearchValues = SearchValues.Create(['K', 'M', 'B', 'T', 'Q']);

        public override string ToString()
            => ExpertCount > 0 ? $"{ExpertCount}x{Count}{ScaleSuffix}" : $"{Count}{ScaleSuffix}";

        public static ModelFileSizeLabel? FromString(ReadOnlySpan<char> sizeLabel)
        {
            if (sizeLabel.Length < 2)
                return null;

            if (!ScaleSuffixesSearchValues.Contains(sizeLabel[^1]))
                return null;

            uint expertCount = 0;
            char scaleSuffix = sizeLabel[^1];
            sizeLabel = sizeLabel[..^1];

            int xIndex = sizeLabel.IndexOf('x');

            if (xIndex > -1)
            {
                var expertsCountSpan = sizeLabel[..xIndex];

                if (!uint.TryParse(expertsCountSpan, out expertCount))
                    return null;

                sizeLabel = sizeLabel[(xIndex + 1)..];
            }

            if (!decimal.TryParse(sizeLabel, out decimal count))
                return null;

            return new ModelFileSizeLabel(expertCount, count, scaleSuffix);
        }
    }
}

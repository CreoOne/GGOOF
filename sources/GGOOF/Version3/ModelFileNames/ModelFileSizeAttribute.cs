using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace GGOOF.Version3.ModelFileNames
{
    public sealed record class ModelFileSizeAttribute(string Name, decimal Count, char ScaleSuffix)
    {
        private static readonly SearchValues<char> ScaleSuffixesSearchValues = SearchValues.Create(['K', 'M', 'B', 'T', 'Q']);

        public override string ToString()
            => $"{Name}{Count}{ScaleSuffix}";

        public static ModelFileSizeAttribute? FromString(ReadOnlySpan<char> sizeLabel)
        {
            if (sizeLabel.Length < 2)
                return null;

            if (!ScaleSuffixesSearchValues.Contains(sizeLabel[^1]))
                return null;

            char scaleSuffix = sizeLabel[^1];
            sizeLabel = sizeLabel[..^1];

            var nIndex = 0;

            while (nIndex < sizeLabel.Length && char.IsLetter(sizeLabel[nIndex]))
                nIndex++;

            if (nIndex == 0 || nIndex == sizeLabel.Length)
                return null;

            var name = sizeLabel[..nIndex].ToString();
            sizeLabel = sizeLabel[nIndex..];

            if (!decimal.TryParse(sizeLabel, out decimal count))
                return null;

            return new ModelFileSizeAttribute(name, count, scaleSuffix);
        }
    }
}

using System.Buffers;
using System.Text;

namespace GGOOF.Version3.ModelFileNames
{
    public sealed record class ModelFileName(string Name, ModelFileSizeLabel? SizeLabel, ModelFileSizeAttribute[] SizeAttributes, string? FineTune, ModelFileVersion? Version, string? Encoding, ModelFileTypeEnum? Type, ModelFileShard? Shard)
    {
        private const string FileExtension = "gguf";
        private const char Separator = '-';
        private const char FileExtensionSeparator = '.';

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Name);

            if (SizeLabel != null)
            {
                builder.Append(Separator);
                builder.Append(SizeLabel);
            }

            foreach (var sizeAttribute in SizeAttributes)
            {
                builder.Append(Separator);
                builder.Append(sizeAttribute);
            }

            if (FineTune != null)
            {
                builder.Append(Separator);
                builder.Append(FineTune);
            }

            if (Version != null)
            {
                builder.Append(Separator);
                builder.Append(Version);
            }

            if (Encoding != null)
            {
                builder.Append(Separator);
                builder.Append(Encoding);
            }

            if (Type != null)
            {
                builder.Append(Separator);
                builder.Append(Type);
            }

            if (Shard != null)
            {
                builder.Append(Separator);
                builder.Append(Shard);
            }

            builder.Append(FileExtensionSeparator);
            builder.Append(FileExtension);
            return builder.ToString();
        }

        /// <summary>
        /// Parses a file name string into a FileName record.
        /// Parser will try its best to extract the components, even if the file name doesn't strictly follow the format specification.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ModelFileName? FromString(ReadOnlySpan<char> s)
        {
            ReadOnlySpan<char> fileNameSpan = s;

            if (fileNameSpan.EndsWith(".gguf", StringComparison.OrdinalIgnoreCase))
                fileNameSpan = fileNameSpan[..^5];

            int expectedMaxParts = fileNameSpan.Count(Separator) + 1;
            Span<Range> partRanges = stackalloc Range[expectedMaxParts];
            int partCount = GetPartRanges(fileNameSpan, partRanges);
            partRanges = partRanges[..partCount];

            int firstKnownIndex = -1;
            int versionIndex = -1;

            ModelFileSizeLabel? sizeLabel = null;
            List<ModelFileSizeAttribute>? sizeAttributes = null;
            ModelFileVersion? version = null;
            ModelFileTypeEnum? type = null;
            ModelFileShard? shard = null;

            Span<bool> processedParts = stackalloc bool[partCount];

            for (int index = 0; index < partCount; index++)
            {
                if (TryConsumeShard(fileNameSpan, partRanges, ref index, out ModelFileShard? parsedShard))
                {
                    shard ??= parsedShard;
                    processedParts[index - 2] = true;
                    processedParts[index - 1] = true;
                    processedParts[index] = true;
                    UpdateFirstKnownIndex(ref firstKnownIndex, index - 2);
                    continue;
                }

                ReadOnlySpan<char> partSpan = fileNameSpan[partRanges[index]];
                bool isProcessed = ProcessSinglePart(partSpan, index, ref sizeLabel, ref sizeAttributes, ref version, ref type, ref versionIndex);

                if (isProcessed)
                {
                    processedParts[index] = true;
                    UpdateFirstKnownIndex(ref firstKnownIndex, index);
                }
            }

            string name = ExtractName(fileNameSpan, partRanges, firstKnownIndex);

            ExtractFineTuneAndEncoding(fileNameSpan, partRanges, processedParts, firstKnownIndex, versionIndex, out string? fineTune, out string? encoding);

            return new ModelFileName(
                name, 
                sizeLabel, 
                sizeAttributes?.ToArray() ?? [], 
                fineTune, 
                version, 
                encoding, 
                type, 
                shard);
        }

        private static int GetPartRanges(ReadOnlySpan<char> fileNameSpan, Span<Range> partRanges)
        {
            int count = 0;

            foreach (Range range in fileNameSpan.Split(Separator))
                partRanges[count++] = range;

            return count;
        }

        private static bool TryConsumeShard(ReadOnlySpan<char> fileNameSpan, ReadOnlySpan<Range> partRanges, ref int currentIndex, out ModelFileShard? shard)
        {
            shard = null;

            if (currentIndex >= partRanges.Length - 2 || !fileNameSpan[partRanges[currentIndex + 1]].Equals("of", StringComparison.OrdinalIgnoreCase))
                return false;

            ReadOnlySpan<char> shardSpan = fileNameSpan[partRanges[currentIndex].Start.Value..partRanges[currentIndex + 2].End.Value];
            shard = ModelFileShard.FromString(shardSpan);

            if (shard == null)
                return false;

            currentIndex += 2;
            return true;
        }

        private static bool ProcessSinglePart(
            ReadOnlySpan<char> partSpan,
            int index,
            ref ModelFileSizeLabel? sizeLabel,
            ref List<ModelFileSizeAttribute>? sizeAttributes,
            ref ModelFileVersion? version,
            ref ModelFileTypeEnum? type,
            ref int versionIndex)
        {
            if (ModelFileSizeLabel.FromString(partSpan) is { } parsedSizeLabel)
            {
                sizeLabel ??= parsedSizeLabel;
                return true;
            }

            if (ParseSizeAttribute(partSpan) is { } parsedSizeAttribute)
            {
                sizeAttributes ??= [];
                sizeAttributes.Add(parsedSizeAttribute);
                return true;
            }

            if (ModelFileVersion.FromString(partSpan) is { } parsedVersion)
            {
                version ??= parsedVersion;
                if (versionIndex == -1) versionIndex = index;
                return true;
            }

            if (ModelFileTypeEnumExtensions.FromString(partSpan) is { } parsedType)
            {
                type ??= parsedType;
                return true;
            }

            return false;
        }

        private static ModelFileSizeAttribute? ParseSizeAttribute(ReadOnlySpan<char> partSpan)
        {
            ModelFileSizeAttribute? attribute = ModelFileSizeAttribute.FromString(partSpan);

            if (attribute != null || partSpan.Length <= 1 || !char.IsAsciiLetterLower(partSpan[^1]))
                return attribute;

            return TryParseCapitalizedSizeAttribute(partSpan);
        }

        private static ModelFileSizeAttribute? TryParseCapitalizedSizeAttribute(ReadOnlySpan<char> partSpan)
        {
            if (partSpan.Length > 128)
                return null;

            Span<char> upperPartSpan = stackalloc char[partSpan.Length];
            partSpan.CopyTo(upperPartSpan);
            upperPartSpan[^1] = char.ToUpperInvariant(partSpan[^1]);

            return ModelFileSizeAttribute.FromString(upperPartSpan);
        }

        private static void UpdateFirstKnownIndex(ref int firstKnownIndex, int currentIndex)
        {
            if (firstKnownIndex == -1)
                firstKnownIndex = currentIndex;
        }

        private static string ExtractName(ReadOnlySpan<char> fileNameSpan, ReadOnlySpan<Range> partRanges, int firstKnownIndex)
        {
            if (firstKnownIndex == -1)
                return JoinRanges(fileNameSpan, partRanges, ' ') ?? string.Empty;

            return JoinRanges(fileNameSpan, partRanges[..firstKnownIndex], ' ') ?? string.Empty;
        }

        private static void ExtractFineTuneAndEncoding(
            ReadOnlySpan<char> fileNameSpan,
            ReadOnlySpan<Range> partRanges,
            ReadOnlySpan<bool> processedParts,
            int firstKnownIndex,
            int versionIndex,
            out string? fineTune,
            out string? encoding)
        {
            fineTune = null;
            encoding = null;

            if (firstKnownIndex == -1)
                return;

            Span<int> unparsedIndices = stackalloc int[partRanges.Length - firstKnownIndex];
            int unparsedCount = 0;

            for (int index = firstKnownIndex; index < partRanges.Length; index++)
                if (!processedParts[index])
                    unparsedIndices[unparsedCount++] = index;

            ReadOnlySpan<int> activeUnparsed = unparsedIndices[..unparsedCount];

            if (activeUnparsed.Length == 0)
                return;

            AssignUnparsedParts(fileNameSpan, partRanges, activeUnparsed, versionIndex, out fineTune, out encoding);
        }

        private static void AssignUnparsedParts(
            ReadOnlySpan<char> fileNameSpan,
            ReadOnlySpan<Range> partRanges,
            ReadOnlySpan<int> unparsedIndices,
            int versionIndex,
            out string? fineTune,
            out string? encoding)
        {
            fineTune = null;
            if (versionIndex != -1)
            {
                int splitPoint = CountIndicesBeforeVersion(unparsedIndices, versionIndex);
                fineTune = JoinRangesIndexed(fileNameSpan, partRanges, unparsedIndices[..splitPoint]);
                encoding = JoinRangesIndexed(fileNameSpan, partRanges, unparsedIndices[splitPoint..]);
            }
            else if (unparsedIndices.Length > 1)
            {
                fineTune = JoinRangesIndexed(fileNameSpan, partRanges, unparsedIndices[..^1]);
                encoding = JoinRangesIndexed(fileNameSpan, partRanges, unparsedIndices[^1..]);
            }
            else
            {
                encoding = JoinRangesIndexed(fileNameSpan, partRanges, unparsedIndices);
            }
        }

        private static int CountIndicesBeforeVersion(ReadOnlySpan<int> unparsedIndices, int versionIndex)
        {
            int count = 0;

            while (count < unparsedIndices.Length && unparsedIndices[count] < versionIndex)
                count++;

            return count;
        }

        private static string? JoinRanges(ReadOnlySpan<char> fileNameSpan, ReadOnlySpan<Range> ranges, char separator)
        {
            if (ranges.Length == 0)
                return null;

            int totalLength = CalculateTotalLength(ranges);
            char[]? rentedBuffer = null;
            Span<char> buffer = totalLength <= 256 ? stackalloc char[totalLength] : (rentedBuffer = ArrayPool<char>.Shared.Rent(totalLength));

            int currentPosition = 0;

            for (int index = 0; index < ranges.Length; index++)
            {
                if (index > 0) buffer[currentPosition++] = separator;
                ReadOnlySpan<char> partSpan = fileNameSpan[ranges[index]];
                partSpan.CopyTo(buffer[currentPosition..]);
                currentPosition += partSpan.Length;
            }

            string result = new(buffer[..totalLength]);

            if (rentedBuffer != null)
                ArrayPool<char>.Shared.Return(rentedBuffer);

            return result;
        }

        private static int CalculateTotalLength(ReadOnlySpan<Range> ranges)
        {
            int length = ranges.Length - 1;

            foreach (Range range in ranges)
                length += range.End.Value - range.Start.Value;

            return length;
        }

        private static string? JoinRangesIndexed(ReadOnlySpan<char> fileNameSpan, ReadOnlySpan<Range> partRanges, ReadOnlySpan<int> indices)
        {
            if (indices.Length == 0)
                return null;

            int totalLength = CalculateTotalLengthIndexed(partRanges, indices);
            char[]? rentedBuffer = null;
            Span<char> buffer = totalLength <= 256 ? stackalloc char[totalLength] : (rentedBuffer = ArrayPool<char>.Shared.Rent(totalLength));

            int currentPosition = 0;

            for (int index = 0; index < indices.Length; index++)
            {
                if (index > 0) buffer[currentPosition++] = Separator;
                ReadOnlySpan<char> partSpan = fileNameSpan[partRanges[indices[index]]];
                partSpan.CopyTo(buffer[currentPosition..]);
                currentPosition += partSpan.Length;
            }

            string result = new(buffer[..totalLength]);

            if (rentedBuffer != null)
                ArrayPool<char>.Shared.Return(rentedBuffer);

            return result;
        }

        private static int CalculateTotalLengthIndexed(ReadOnlySpan<Range> ranges, ReadOnlySpan<int> indices)
        {
            int length = indices.Length - 1;

            foreach (int index in indices)
            {
                Range range = ranges[index];
                length += range.End.Value - range.Start.Value;
            }

            return length;
        }
    }
}

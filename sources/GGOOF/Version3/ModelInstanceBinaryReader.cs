using GGOOF.Version3.MetadataDictionaries;
using GGOOF.Version3.Spec;
using System.Buffers.Binary;
using System.Text;

namespace GGOOF.Version3
{
    internal sealed class ModelInstanceBinaryReader
    {
        private readonly static byte[] MagicNumbers = [0x47, 0x47, 0x55, 0x46]; // "GGUF"
        private const uint Version = 3;

        internal static ModelInstance ReadFromStream(Stream modelStream)
        {
            using var reader = new BinaryReader(modelStream, Encoding.UTF8, leaveOpen: true);
            Span<byte> magicNumbers = stackalloc byte[MagicNumbers.Length];

            if (modelStream.Read(magicNumbers) != MagicNumbers.Length || !magicNumbers.SequenceEqual(MagicNumbers))
                throw new InvalidDataException("Invalid magic numbers. Expected 'GGUF'.");
            
            var version = reader.ReadUInt64();

            if (version != Version)
                throw new InvalidDataException($"Unsupported version. Expected {Version}, got {version}.");

            var tensorCount = reader.ReadUInt64();
            var metadataKeyValuePairCount = reader.ReadUInt64();
            var metadata = new GenericMultiTypeMetadataDictionary();

            ReadMetadata(reader, metadata, metadataKeyValuePairCount);

            return new()
            {
                TensorCount = tensorCount,
                Metadata = metadata
            };
        }

        private static void ReadMetadata(BinaryReader reader, GenericMultiTypeMetadataDictionary metadata, ulong metadataKeyValuePairCount)
        {
            while (metadataKeyValuePairCount-- > 0)
            {
                var key = ReadString(reader, Encoding.ASCII);
                var valueType = (MetadataValueTypeEnum)reader.ReadUInt64();

                switch (valueType)
                {
                    case MetadataValueTypeEnum.GGUF_METADATA_VALUE_TYPE_STRING:
                        metadata.Set(key, ReadString(reader, Encoding.UTF8));
                        break;
                    case MetadataValueTypeEnum.GGUF_METADATA_VALUE_TYPE_UINT64:
                        metadata.Set(key, reader.ReadUInt64());
                        break;
                    case MetadataValueTypeEnum.GGUF_METADATA_VALUE_TYPE_INT64:
                        metadata.Set(key, reader.ReadInt64());
                        break;
                    case MetadataValueTypeEnum.GGUF_METADATA_VALUE_TYPE_FLOAT32:
                        metadata.Set(key, reader.ReadSingle());
                        break;
                    case MetadataValueTypeEnum.GGUF_METADATA_VALUE_TYPE_FLOAT64:
                        metadata.Set(key, reader.ReadDouble());
                        break;
                    case MetadataValueTypeEnum.GGUF_METADATA_VALUE_TYPE_BOOL:
                        metadata.Set(key, reader.ReadByte() != 0);
                        break;
                    case MetadataValueTypeEnum.GGUF_METADATA_VALUE_TYPE_ARRAY:
                        throw new NotImplementedException();
                    default:
                        throw new InvalidDataException($"Unsupported metadata value type: {valueType}");
                }
            }
        }

        private static string ReadString(BinaryReader reader, Encoding encoding)
        {
            var byteLength = (int)reader.ReadUInt64();
            var bytes = reader.ReadBytes(byteLength);
            return encoding.GetString(bytes);
        }
    }
}

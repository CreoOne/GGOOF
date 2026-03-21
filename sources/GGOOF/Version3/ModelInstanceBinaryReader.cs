namespace GGOOF.Version3
{
    internal sealed class ModelInstanceBinaryReader
    {
        private readonly static byte[] MagicNumbers = [0x47, 0x47, 0x55, 0x46]; // "GGUF"
        private const uint Version = 3;

        internal static ModelInstance ReadFromStream(Stream modelStream)
        {
            throw new NotImplementedException();
        }
    }
}

using GGOOF.Version3.ModelFileNames;

namespace GGOOF.Version3
{
    public sealed class ModelFactory
    {
        public ModelInstance FromStream(Stream modelStream)
            => ModelInstanceBinaryReader.ReadFromStream(modelStream);

        public ModelInstance FromFile(string modelPath)
        {
            using var modelStream = File.OpenRead(modelPath);
            return FromStream(modelStream);
        }

        public ModelInstance FromFile(ModelFileName fileName)
            => FromFile(fileName.ToString());

        public ModelInstance FromScratch()
            => new();
    }
}

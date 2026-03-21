namespace GGOOF.Version3
{
    public sealed class ModelInstance
    {
        public readonly uint Version = 3;
        public ulong TensorCount { get; set; }
        public ulong MetadataKeyValuePairCount { get; }
    }
}

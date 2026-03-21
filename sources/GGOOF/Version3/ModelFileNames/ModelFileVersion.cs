namespace GGOOF.Version3.ModelFileNames
{
    public sealed record class ModelFileVersion(ulong Major, ulong Minor)
    {
        public static readonly ModelFileVersion Default = new(1, 0);

        public static bool IsDefault(ModelFileVersion version)
            => Default == version;

        public override string ToString()
            => $"v{Major}.{Minor}";

        public static ModelFileVersion? FromString(ReadOnlySpan<char> s)
        {
            if (!s.StartsWith('v'))
                return null;

            var noV = s.Slice(1);
            var parts = noV.Split('.');
            var major = 0ul;
            var minor = 0ul;

            for (var index = 0; index < 2 && parts.MoveNext(); index++)
            {
                var part = noV[parts.Current.Start.Value..parts.Current.End.Value];

                if (!ulong.TryParse(part, out ulong value))
                    return null;

                if (index == 0)
                    major = value;

                if (index == 1)
                    minor = value;
            }

            return new ModelFileVersion(major, minor);
        }
    }
}

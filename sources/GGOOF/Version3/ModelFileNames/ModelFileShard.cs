namespace GGOOF.Version3.ModelFileNames
{
    public sealed record class ModelFileShard(uint Index, uint Count)
    {
        public override string ToString()
            => $"{Index:00000}-of-{Count:00000}";

        public static ModelFileShard? FromString(ReadOnlySpan<char> s)
        {
            var parts = s.Split("-of-");

            uint index = 0;
            uint count = 0;

            for (var i = 0; i < 2 && parts.MoveNext(); i++)
            {
                var part = s[parts.Current.Start.Value..parts.Current.End.Value];

                if (!uint.TryParse(part, out uint value))
                    return null;

                if (i == 0)
                    index = value;

                if (i == 1)
                    count = value;
            }

            return new ModelFileShard(index, count);
        }
    }
}

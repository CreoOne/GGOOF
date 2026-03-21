namespace GGOOF.Validations
{
    public readonly record struct ValidationError(ulong Code, string Description, Dictionary<string, string> Properties)
    {
        public ValidationError(ulong code, string description)
            : this(code, description, [])
        { }
    }
}

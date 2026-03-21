namespace GGOOF.Validations
{
    public readonly record struct ValidationResult(IEnumerable<ValidationError> Errors)
    {
        public bool IsValid => Errors is not null && Errors.Any();
    }
}

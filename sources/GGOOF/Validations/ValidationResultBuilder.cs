namespace GGOOF.Validations
{
    internal sealed class ValidationResultBuilder
    {
        private readonly List<ValidationError> _errors = [];

        public ValidationResultBuilder AddError(ValidationError error)
        {
            _errors.Add(error);
            return this;
        }

        public ValidationResult Build()
            => new(_errors);
    }
}

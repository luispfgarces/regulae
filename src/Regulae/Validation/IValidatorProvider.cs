namespace Regulae.Validation
{
    using FluentValidation;

    internal interface IValidatorProvider
    {
        IValidator<T> GetValidatorFor<T>();
    }
}
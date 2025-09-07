namespace Regulae.Validation
{
    using System;
    using System.Collections.Generic;
    using FluentValidation;

    internal sealed class ValidationProvider : IValidatorProvider
    {
        private readonly Dictionary<Type, IValidator> validatorsByType;

        private ValidationProvider()
        {
            this.validatorsByType = new Dictionary<Type, IValidator>();
        }

        public static ValidationProvider New()
            => new ValidationProvider();

        public IValidator<T> GetValidatorFor<T>()
            => this.validatorsByType.TryGetValue(typeof(T), out var validator)
                ? (IValidator<T>)validator
                : throw new NotSupportedException($"No validator for type '{typeof(T).Name}' exists.");

        public ValidationProvider MapValidatorFor<T>(IValidator<T> validator)
        {
            this.validatorsByType[typeof(T)] = validator;
            return this;
        }
    }
}
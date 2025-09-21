namespace Regulae
{
    using System;
    using System.Collections.Generic;

    internal static class Operation
    {
        internal static OperationResult Failure(OperationError error) => Failure([error]);

        internal static OperationResult Failure(IList<OperationError> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);
            return new OperationResult(errors: errors);
        }

        internal static OperationResult Success() => new(errors: []);
    }
}

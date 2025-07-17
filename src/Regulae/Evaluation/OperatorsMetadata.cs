namespace Regulae.Evaluation
{
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using Regulae;

    internal static class OperatorsMetadata
    {
        private static readonly FrozenDictionary<Operators, OperatorMetadata> allOperatorsMetadata;

        private static readonly IEnumerable<OperatorMetadata> operatorsMetadata = new[]
        {
            Equal,
            NotEqual,
            GreaterThan,
            GreaterThanOrEqual,
            LesserThan,
            LesserThanOrEqual,
            Contains,
            NotContains,
            In,
            NotIn,
            StartsWith,
            EndsWith,
            CaseInsensitiveStartsWith,
            CaseInsensitiveEndsWith,
            NotStartsWith,
            NotEndsWith,
        };

        static OperatorsMetadata()
        {
            var operatorsMetadataByOperator = new Dictionary<Operators, OperatorMetadata>();
            var allOperatorsMetadataBySupportedCombinationAux = new Dictionary<string, OperatorMetadata>(StringComparer.Ordinal);

            foreach (var operatorMetadata in operatorsMetadata)
            {
                operatorsMetadataByOperator.Add(operatorMetadata.Operator, operatorMetadata);
            }

            allOperatorsMetadata = operatorsMetadataByOperator.ToFrozenDictionary();
        }

        public static IEnumerable<OperatorMetadata> All => allOperatorsMetadata.Values;

        public static IDictionary<Operators, OperatorMetadata> AllByOperator => allOperatorsMetadata;

        public static OperatorMetadata CaseInsensitiveEndsWith => new(Operators.CaseInsensitiveEndsWith, Multiplicities.OneToOne);

        public static OperatorMetadata CaseInsensitiveStartsWith => new(Operators.CaseInsensitiveStartsWith, Multiplicities.OneToOne);

        public static OperatorMetadata Contains => new(Operators.Contains, Multiplicities.OneToOne, Multiplicities.ManyToOne);

        public static OperatorMetadata EndsWith => new(Operators.EndsWith, Multiplicities.OneToOne);

        public static OperatorMetadata Equal => new(Operators.Equal, Multiplicities.OneToOne);

        public static OperatorMetadata GreaterThan => new(Operators.GreaterThan, Multiplicities.OneToOne);

        public static OperatorMetadata GreaterThanOrEqual => new(Operators.GreaterThanOrEqual, Multiplicities.OneToOne);

        public static OperatorMetadata In => new(Operators.In, Multiplicities.OneToMany);

        public static OperatorMetadata LesserThan => new(Operators.LesserThan, Multiplicities.OneToOne);

        public static OperatorMetadata LesserThanOrEqual => new(Operators.LesserThanOrEqual, Multiplicities.OneToOne);

        public static OperatorMetadata NotContains => new(Operators.NotContains, Multiplicities.OneToOne);

        public static OperatorMetadata NotEndsWith => new(Operators.NotEndsWith, Multiplicities.OneToOne);

        public static OperatorMetadata NotEqual => new(Operators.NotEqual, Multiplicities.OneToOne);

        public static OperatorMetadata NotIn => new(Operators.NotIn, Multiplicities.OneToMany);

        public static OperatorMetadata NotStartsWith => new(Operators.NotStartsWith, Multiplicities.OneToOne);

        public static OperatorMetadata StartsWith => new(Operators.StartsWith, Multiplicities.OneToOne);
    }
}
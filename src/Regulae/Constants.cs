namespace Regulae
{
    internal static class Constants
    {
        public const int DefaultPropertiesDictionarySize = 2;

        internal static class ErrorCodes
        {
            /// <summary>
            /// Rule name must be not null, empty, or whitespace.
            /// </summary>
            public const string R0001 = "R0001";

            /// <summary>
            /// Rule ruleset must be not null, empty, or whitespace.
            /// </summary>
            public const string R0002 = "R0002";

            /// <summary>
            /// Rule date begin must not have the default value for a date (0001-01-01).
            /// </summary>
            public const string R0003 = "R0003";

            /// <summary>
            /// Rule date end (if defined) must greater or equal to the date begin.
            /// </summary>
            public const string R0004 = "R0004";

            /// <summary>
            /// Rule content must be defined (even if content is a null value).
            /// </summary>
            public const string R0005 = "R0005";

            /// <summary>
            /// Rule ruleset must exist (should be already created).
            /// </summary>
            public const string R0006 = "R0006";

            /// <summary>
            /// Rule name must be unique per ruleset.
            /// </summary>
            public const string R0007 = "R0007";

            /// <summary>
            /// Rule to be updated must exist.
            /// </summary>
            public const string R0008 = "R0008";

            /// <summary>
            /// Composed condition logical operator must be an And, an Or, or a Xor.
            /// </summary>
            public const string R0009 = "R0009";

            /// <summary>
            /// Composed condition must have at least 2 child condition nodes.
            /// </summary>
            public const string R0010 = "R0010";

            /// <summary>
            /// Composed condition can only include composed conditions and value conditions as child condition nodes.
            /// </summary>
            public const string R0011 = "R0011";

            /// <summary>
            /// Value condition node condition name must be not null, empty, or whitespace.
            /// </summary>
            public const string R0012 = "R0012";

            /// <summary>
            /// Value condition node operator must be a supported operator.
            /// </summary>
            public const string R0013 = "R0013";

            /// <summary>
            /// Value condition node operator must be one of the following when the right operand data type is string and cardinality is one: Equal, NotEqual, Contains, NotContains, StartsWith, EndsWith, CaseInsensitiveStartsWith, CaseInsensitiveEndsWith, NotStartsWith, and NotEndsWith.
            /// </summary>
            public const string R0014 = "R0014";

            /// <summary>
            /// Value condition node operator must be one of the following when the right operand data type is boolean and cardinality is one: Equal and NotEqual.
            /// </summary>
            public const string R0015 = "R0015";

            /// <summary>
            /// Value condition node operator must be one of the following when the right operand data type is integer and cardinality is one: Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LesserThan, and LesserThanOrEqual.
            /// </summary>
            public const string R0016 = "R0016";

            /// <summary>
            /// Value condition node operator must be one of the following when the right operand data type is decimal and cardinality is one: Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LesserThan, and LesserThanOrEqual.
            /// </summary>
            public const string R0017 = "R0017";

            /// <summary>
            /// Value condition node operator must be one of the following when the right operand data type is string and cardinality is many: In, NotIn, Equal, and NotEqual.
            /// </summary>
            public const string R0018 = "R0018";

            /// <summary>
            /// Value condition node operator must be one of the following when the right operand data type is boolean and cardinality is many: In, NotIn, Equal, and NotEqual.
            /// </summary>
            public const string R0019 = "R0019";

            /// <summary>
            /// Value condition node operator must be one of the following when the right operand data type is integer and cardinality is many: In, NotIn, Equal, and NotEqual.
            /// </summary>
            public const string R0020 = "R0020";

            /// <summary>
            /// Value condition node operator must be one of the following when the right operand data type is decimal and cardinality is many: In, NotIn, Equal, and NotEqual.
            /// </summary>
            public const string R0021 = "R0021";

            /// <summary>
            /// A rule add priority option must be specified when adding a new rule.
            /// </summary>
            public const string R0022 = "R0022";

            /// <summary>
            /// The "at number option value" must be specified when rule add priority option "AtNumber" was specified.
            /// </summary>
            public const string R0023 = "R0023";

            /// <summary>
            /// The "at rule name option value" must be specified when rule add priority option "AtRulename" was specified.
            /// </summary>
            public const string R0024 = "R0024";

            /// <summary>
            /// The "at rule name option value" must match an existent rule name when rule add priority option "AtRulename" was specified.
            /// </summary>
            public const string R0025 = "R0025";

            /// <summary>
            /// The condition right operand value does not comply with the defined data type for it.
            /// </summary>
            public const string R0026 = "R0026";

            /// <summary>
            /// The condition right operand defines a value that is not convertible to the data type associated with the condition.
            /// </summary>
            public const string R0027 = "R0027";

            /// <summary>
            /// The condition right operand value was expected to be an enumeration of values but instead a single value was specified.
            /// </summary>
            public const string R0028 = "R0028";
        }
    }
}
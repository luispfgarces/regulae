namespace Regulae.Providers.InMemory.DataModel
{
    using Regulae;

    internal sealed class ValueConditionNodeDataModel : ConditionNodeDataModel
    {
        public required string Condition { get; set; }

        public required Operators Operator { get; set; }

        public required Operand RightOperand { get; set; }
    }
}
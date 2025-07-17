namespace Regulae.Providers.InMemory.DataModel
{
    using Regulae;

    internal sealed class ValueConditionNodeDataModel : ConditionNodeDataModel
    {
        public string Condition { get; set; }

        public Operators Operator { get; set; }

        public Operand RightOperand { get; set; }
    }
}
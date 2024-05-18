namespace Regulae.Providers.InMemory.DataModel
{
    internal sealed class ComposedConditionNodeDataModel : ConditionNodeDataModel
    {
        public ConditionNodeDataModel[] ChildConditionNodes { get; set; }
    }
}
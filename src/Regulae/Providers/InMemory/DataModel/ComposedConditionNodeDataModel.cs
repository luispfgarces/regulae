namespace Regulae.Providers.InMemory.DataModel
{
    internal sealed class ComposedConditionNodeDataModel : ConditionNodeDataModel
    {
        public required ConditionNodeDataModel[] ChildConditionNodes { get; set; }
    }
}
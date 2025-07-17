namespace Regulae.IntegrationTests.Common.Scenarios.Scenario5
{
    public enum BestServerConditions : byte
    {
        [DataType(DataTypes.String)]
        Brand = 1,

        [DataType(DataTypes.Decimal)]
        Price = 2,

        [DataType(DataTypes.Integer)]
        Memory = 3,

        [DataType(DataTypes.Boolean)]
        StoragePartionable = 4,

        [DataType(DataTypes.Integer)]
        Storage = 5
    }
}
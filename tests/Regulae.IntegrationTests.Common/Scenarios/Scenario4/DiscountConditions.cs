namespace Regulae.IntegrationTests.Common.Scenarios.Scenario4
{
    public enum DiscountConditions
    {
        [DataType(DataTypes.String)]
        ProductBrand = 1,

        [DataType(DataTypes.Decimal)]
        ProductRecommendedRetailPrice = 2,

        [DataType(DataTypes.Integer)]
        ProductTier = 3,

        [DataType(DataTypes.String)]
        ProductColor = 4,

        [DataType(DataTypes.String)]
        CustomerEmail = 5,
    }
}
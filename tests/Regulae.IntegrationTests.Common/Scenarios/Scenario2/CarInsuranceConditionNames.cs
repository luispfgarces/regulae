namespace Regulae.IntegrationTests.Common.Scenarios.Scenario2
{
    public enum CarInsuranceConditionNames
    {
        [DataType(DataTypes.Decimal)]
        RepairCosts = 1,

        [DataType(DataTypes.Decimal)]
        RepairCostsCommercialValueRate = 2,

        [DataType(DataTypes.Decimal)]
        SelfDamageCoverage = 3,

        [DataType(DataTypes.String)]
        ClaimDescription = 4
    }
}
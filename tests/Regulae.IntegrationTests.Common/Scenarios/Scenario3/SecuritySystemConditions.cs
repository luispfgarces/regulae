namespace Regulae.IntegrationTests.Common.Scenarios.Scenario3
{
    public enum SecuritySystemConditions
    {
        [DataType(DataTypes.Decimal)]
        TemperatureCelsius = 1,

        [DataType(DataTypes.Decimal)]
        SmokeRate = 2,

        [DataType(DataTypes.String)]
        PowerStatus = 3
    }
}
namespace Regulae.Tests.ConditionNodes
{
    using FluentAssertions;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class ValueConditionNodeTests
    {
        [Fact]
        public void Clone_BooleanDataType_ReturnsCloneInstance()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperandValue = false;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedOperandDataType = DataTypes.Boolean;
            var expectedOperandCardinality = Cardinalities.One;

            var sut = new ValueConditionNode(expectedCondition, expectedOperator, new Operand(expectedOperandValue));
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode>();
            var valueConditionNode = actual.As<ValueConditionNode>();
            valueConditionNode.Condition.Should().Be(expectedCondition);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.RightOperand.Cardinality.Should().Be(expectedOperandCardinality);
            valueConditionNode.RightOperand.DataType.Should().Be(expectedOperandDataType);
            valueConditionNode.RightOperand.Value.Should().Be(expectedOperandValue);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void Clone_DecimalDataType_ReturnsCloneInstance()
        {
            // Arrange
            var expectedCondition = ConditionNames.PluviosityRate.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 5682.2654m;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Decimal;
            var expectedOperandCardinality = Cardinalities.One;

            var sut = new ValueConditionNode(expectedCondition, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode>();
            var valueConditionNode = actual.As<ValueConditionNode>();
            valueConditionNode.Condition.Should().Be(expectedCondition);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.RightOperand.Cardinality.Should().Be(expectedOperandCardinality);
            valueConditionNode.RightOperand.DataType.Should().Be(expectedDataType);
            valueConditionNode.RightOperand.Value.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void Clone_IntegerDataType_ReturnsCloneInstance()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 1616;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Integer;
            var expectedOperandCardinality = Cardinalities.One;

            var sut = new ValueConditionNode(expectedCondition, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode>();
            var valueConditionNode = actual.As<ValueConditionNode>();
            valueConditionNode.Condition.Should().Be(expectedCondition);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.RightOperand.Cardinality.Should().Be(expectedOperandCardinality);
            valueConditionNode.RightOperand.DataType.Should().Be(expectedDataType);
            valueConditionNode.RightOperand.Value.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void Clone_StringDataType_ReturnsCloneInstance()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = "Such operand, much wow.";
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.String;
            var expectedOperandCardinality = Cardinalities.One;

            var sut = new ValueConditionNode(expectedCondition, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode>();
            var valueConditionNode = actual.As<ValueConditionNode>();
            valueConditionNode.Condition.Should().Be(expectedCondition);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.RightOperand.Cardinality.Should().Be(expectedOperandCardinality);
            valueConditionNode.RightOperand.DataType.Should().Be(expectedDataType);
            valueConditionNode.RightOperand.Value.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void Init_GivenSetupWithBooleanValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = false;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Boolean;
            var expectedOperandCardinality = Cardinalities.One;

            var sut = new ValueConditionNode(expectedCondition, expectedOperator, expectedOperand);

            // Act
            var actualCondition = sut.Condition;
            var actualOperator = sut.Operator;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualCardinality = sut.RightOperand.Cardinality;
            var actualDataType = sut.RightOperand.DataType;
            var actualOperand = sut.RightOperand.Value;

            // Assert
            actualCondition.Should().Be(expectedCondition);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
            actualCardinality.Should().Be(expectedOperandCardinality);
        }

        [Fact]
        public void Init_GivenSetupWithDecimalValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedCondition = ConditionNames.PluviosityRate.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 5682.2654m;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Decimal;
            var expectedOperandCardinality = Cardinalities.One;

            var sut = new ValueConditionNode(expectedCondition, expectedOperator, expectedOperand);

            // Act
            var actualCondition = sut.Condition;
            var actualOperator = sut.Operator;
            var actualCardinality = sut.RightOperand.Cardinality;
            var actualDataType = sut.RightOperand.DataType;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualOperand = sut.RightOperand.Value;

            // Assert
            actualCondition.Should().Be(expectedCondition);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
            actualCardinality.Should().Be(expectedOperandCardinality);
        }

        [Fact]
        public void Init_GivenSetupWithIntegerValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 1616;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Integer;
            var expectedOperandCardinality = Cardinalities.One;

            var sut = new ValueConditionNode(expectedCondition, expectedOperator, expectedOperand);

            // Act
            var actualCondition = sut.Condition;
            var actualOperator = sut.Operator;
            var actualCardinality = sut.RightOperand.Cardinality;
            var actualDataType = sut.RightOperand.DataType;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualOperand = sut.RightOperand.Value;

            // Assert
            actualCondition.Should().Be(expectedCondition);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
            actualCardinality.Should().Be(expectedOperandCardinality);
        }

        [Fact]
        public void Init_GivenSetupWithStringValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = "Such operand, much wow.";
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.String;
            var expectedOperandCardinality = Cardinalities.One;

            var sut = new ValueConditionNode(expectedCondition, expectedOperator, expectedOperand);

            // Act
            var actualCondition = sut.Condition;
            var actualOperator = sut.Operator;
            var actualCardinality = sut.RightOperand.Cardinality;
            var actualDataType = sut.RightOperand.DataType;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualOperand = sut.RightOperand.Value;

            // Assert
            actualCondition.Should().Be(expectedCondition);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
            actualCardinality.Should().Be(expectedOperandCardinality);
        }
    }
}
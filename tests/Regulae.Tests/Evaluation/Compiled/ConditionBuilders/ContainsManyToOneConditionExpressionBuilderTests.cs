namespace Regulae.Tests.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled.ConditionBuilders;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ContainsManyToOneConditionExpressionBuilderTests
    {
        private readonly ContainsManyToOneConditionExpressionBuilder conditionExpressionBuilder;

        public ContainsManyToOneConditionExpressionBuilderTests()
        {
            this.conditionExpressionBuilder = new ContainsManyToOneConditionExpressionBuilder();
        }

        public static IEnumerable<object[]> ValidCases => new[]
        {
            new object[]{ DataTypes.Boolean, typeof(IEnumerable<bool>), typeof(bool), new[] { true, }, new[] { false, }, true },
            new object[]{ DataTypes.Decimal, typeof(IEnumerable<decimal>), typeof(decimal), new[] { 10.5m, 3.6m, 1.9m, }, new[] { 2.4m, 5.6m, 7.0m, }, 10.5m },
            new object[]{ DataTypes.Integer, typeof(IEnumerable<int>), typeof(int), new[] { 1, 2, 3, 4, 5, }, new[] { 10, 11, 12, 13, 14, }, 3 },
            new object[]{ DataTypes.String, typeof(IEnumerable<string>), typeof(string), new[] { "A", "B", "C", "D", }, new[] { "E", "F", "G", "H", }, "C" },
        };

        [Theory]
        [MemberData(nameof(ValidCases))]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ReturnsConditionExpression(
            DataTypes dataType,
            Type leftOperandType,
            Type rightOperandType,
            object leftOperandMatch,
            object leftOperandNonMatch,
            object rightOperand)
        {
            // Act
            var expressionResult = ExpressionBuilder.NewExpression("TestCondition")
                .WithParameters(p =>
                {
                    p.CreateParameter("leftHand", typeof(object));
                })
                .HavingReturn(typeof(bool), false)
                .SetImplementation(builder =>
                {
                    var args = new BuildConditionExpressionArgs
                    {
                        DataTypeConfiguration = DataTypeConfiguration.Create(dataType, rightOperandType, null),
                        LeftHandOperand = builder.ConvertChecked(builder.GetParameter("leftHand"), leftOperandType),
                        RightHandOperand = builder.ConvertChecked(builder.Constant(rightOperand), rightOperandType),
                    };
                    var conditionExpression = this.conditionExpressionBuilder
                        .BuildConditionExpression(builder, args);

                    builder.Return(conditionExpression);
                })
                .Build();

            // Assert
            var actualExpression = expressionResult.Implementation;
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<object, bool>>(actualExpression, expressionResult.Parameters).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke(leftOperandMatch);
            var notNullLeftHandValueResult2 = compiledExpression.Invoke(leftOperandNonMatch);

            notNullLeftHandValueResult1.Should().BeTrue();
            notNullLeftHandValueResult2.Should().BeFalse();
        }
    }
}
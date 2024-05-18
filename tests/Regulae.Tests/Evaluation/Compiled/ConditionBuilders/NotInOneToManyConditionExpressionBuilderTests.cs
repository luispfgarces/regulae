namespace Regulae.Tests.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;
    using Regulae.Evaluation.Compiled.ConditionBuilders;
    using Xunit;

    public class NotInOneToManyConditionExpressionBuilderTests
    {
        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForInt_ReturnsConditionExpression()
        {
            // Arrange
            var notInOneToManyConditionExpressionBuilder
                = new NotInOneToManyConditionExpressionBuilder();

            // Act
            var expressionResult = ExpressionBuilder.NewExpression("TestCondition")
                .WithParameters(p =>
                {
                    p.CreateParameter("leftHand", typeof(int));
                })
                .HavingReturn(typeof(bool), false)
                .SetImplementation(builder =>
                {
                    var args = new BuildConditionExpressionArgs
                    {
                        DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0),
                        LeftHandOperand = builder.GetParameter("leftHand"),
                        RightHandOperand = builder.Constant<IEnumerable<int>>(new[] { 1, 2, 3 }),
                    };
                    var conditionExpression = notInOneToManyConditionExpressionBuilder
                        .BuildConditionExpression(builder, args);

                    builder.Return(conditionExpression);
                })
                .Build();

            // Assert
            var actualExpression = expressionResult.Implementation;
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<int, bool>>(actualExpression, expressionResult.Parameters).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke(2);
            var notNullLeftHandValueResult2 = compiledExpression.Invoke(0);

            notNullLeftHandValueResult1.Should().BeFalse();
            notNullLeftHandValueResult2.Should().BeTrue();
        }
    }
}
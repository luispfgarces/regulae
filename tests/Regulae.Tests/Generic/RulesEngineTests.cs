namespace Regulae.Tests.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Generic;
    using Regulae.Tests.TestStubs;

    using Xunit;

    public class RulesEngineTests
    {
        private readonly IRulesEngine rulesEngineMock;

        public RulesEngineTests()
        {
            this.rulesEngineMock = Mock.Of<IRulesEngine>();
        }

        [Fact]
        public async Task GetRulesetsAsync_NoConditionsGiven_ReturnsRulesets()
        {
            // Arrange
            var ruleset1 = new Ruleset("Type1", DateTime.UtcNow);
            var ruleset2 = new Ruleset("Type2", DateTime.UtcNow);
            var rulesets = new Dictionary<string, Ruleset>
            {
                { ruleset1.Name, ruleset1 },
                { ruleset2.Name, ruleset2 },
            };
            var expectedGenericRulesets = new Dictionary<RulesetNames, Ruleset<RulesetNames>>
            {
                { RulesetNames.Type1, new Ruleset<RulesetNames>(ruleset1) },
                { RulesetNames.Type2, new Ruleset<RulesetNames>(ruleset2) },
            };
            Mock.Get(this.rulesEngineMock)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(rulesets);

            var genericRulesEngine = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var genericRulesets = await genericRulesEngine.GetRulesetsAsync();

            // Assert
            genericRulesets.Should().BeEquivalentTo(expectedGenericRulesets);
        }

        [Fact]
        public async Task GetRulesetsAsync_WithEmptyRulesetsNames_ReturnsEmptyRulesetsCollection()
        {
            // Arrange
            Mock.Get(this.rulesEngineMock)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(new Dictionary<string, Ruleset>());
            var genericRulesEngine = new RulesEngine<EmptyRulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var genericRulesets = await genericRulesEngine.GetRulesetsAsync();

            // Assert
            genericRulesets.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUniqueConditions_GivenRulesetAndDatesInterval_ReturnsConditions()
        {
            // Arrange
            Mock.Get(this.rulesEngineMock)
                .Setup(x => x.GetUniqueConditionsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[] { nameof(ConditionNames.NumberOfSales), nameof(ConditionNames.IsVip), });

            var genericRulesEngine = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var genericConditions = await genericRulesEngine.GetUniqueConditionsAsync(RulesetNames.Type1, DateTime.MinValue, DateTime.MaxValue);

            // Assert
            genericConditions.Should().NotBeNullOrEmpty()
                .And.Contain(ConditionNames.NumberOfSales)
                .And.Contain(ConditionNames.IsVip);
        }

        [Fact]
        public void Options_PropertyGet_ReturnsRulesEngineOptions()
        {
            // Arrange
            var options = RulesEngineOptions.NewWithDefaults();
            Mock.Get(this.rulesEngineMock)
                .SetupGet(x => x.Options)
                .Returns(options);

            var genericRulesEngine = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var actual = genericRulesEngine.Options;

            // Assert
            actual.Should().BeSameAs(options);
        }

        [Fact]
        public async Task SearchAsync_GivenRulesetAndDatesIntervalAndNoConditions_ReturnsRules()
        {
            // Arrange
            var expectedRule = Rule.Create<RulesetNames, ConditionNames>("Test rule")
                .InRuleset(RulesetNames.Type1)
                .SetContent(new object())
                .Since(new DateTime(2018, 01, 01))
                .Until(new DateTime(2019, 01, 01))
                .ApplyWhen(ConditionNames.IsoCountryCode, Operators.Equal, "USA")
                .Build().Rule;
            expectedRule.Priority = 3;

            var dateBegin = new DateTime(2022, 01, 01);
            var dateEnd = new DateTime(2022, 12, 01);
            var genericRuleset = RulesetNames.Type1;

            var genericSearchArgs = new SearchArgs<RulesetNames, ConditionNames>(genericRuleset, dateBegin, dateEnd);

            var testRule = Rule.Create<RulesetNames, ConditionNames>("Test rule")
                .InRuleset(RulesetNames.Type1)
                .SetContent(new object())
                .Since(new DateTime(2018, 01, 01))
                .Until(new DateTime(2019, 01, 01))
                .ApplyWhen(ConditionNames.IsoCountryCode, Operators.Equal, "USA")
                .Build().Rule;
            testRule.Priority = 3;
            var testRules = new List<Rule>
            {
                testRule
            };

            Mock.Get(this.rulesEngineMock)
                .Setup(m => m.SearchAsync(It.IsAny<SearchArgs<string, string>>()))
                .ReturnsAsync(testRules);

            var genericRulesEngine = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var genericRules = await genericRulesEngine.SearchAsync(genericSearchArgs);

            // Assert
            var actualRule = genericRules.First();
            actualRule.Should().BeEquivalentTo(expectedRule);
            Mock.Get(this.rulesEngineMock)
                .Verify(m => m.SearchAsync(It.IsAny<SearchArgs<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.ActivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.AddRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.DeactivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.SearchAsync), "searchArgs", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine<RulesetNames, ConditionNames>.UpdateRuleAsync), "rule", typeof(ArgumentNullException))]
        public async Task VerifyParameters_GivenNullParameter_ThrowsArgumentNullException(string methodName, string parameterName, Type exceptionType)
        {
            // Arrange
            var sut = new RulesEngine<RulesetNames, ConditionNames>(this.rulesEngineMock);

            // Act
            var actual = await Assert.ThrowsAsync(exceptionType, async () =>
            {
                switch (methodName)
                {
                    case nameof(RulesEngine<RulesetNames, ConditionNames>.ActivateRuleAsync):
                        _ = await sut.ActivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine<RulesetNames, ConditionNames>.AddRuleAsync):
                        _ = await sut.AddRuleAsync(null, RuleAddPriorityOption.AtSmallestNumber);
                        break;

                    case nameof(RulesEngine<RulesetNames, ConditionNames>.DeactivateRuleAsync):
                        _ = await sut.DeactivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine<RulesetNames, ConditionNames>.SearchAsync):
                        _ = await sut.SearchAsync(null);
                        break;

                    case nameof(RulesEngine<RulesetNames, ConditionNames>.UpdateRuleAsync):
                        _ = await sut.UpdateRuleAsync(null);
                        break;

                    default:
                        Assert.Fail("Test scenario not supported, please review test implementation to support it.");
                        break;
                }
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType(exceptionType);
            if (actual is ArgumentException argumentException)
            {
                argumentException.Message.Should().Contain(parameterName);
                argumentException.ParamName.Should().Be(parameterName);
            }
        }


        [Fact]
        public async Task ActivateRuleAsync_CallsWrappedEngineAndReturnsResult()
        {
            // Arrange
            var wrapped = new Mock<IRulesEngine>();

            var genericRule = Rule.Create<RulesetNames, ConditionNames>("r1")
                .InRuleset(RulesetNames.Type1)
                .SetContent(new object())
                .Since(DateTime.UtcNow)
                .Build()
                .Rule;

            Mock.Get(wrapped.Object)
                .Setup(w => w.ActivateRuleAsync(It.IsAny<Rule>()))
                .ReturnsAsync(Operation.Success())
                .Verifiable();

            var sut = new RulesEngine<RulesetNames, ConditionNames>(wrapped.Object);

            // Act
            var result = await sut.ActivateRuleAsync(genericRule);

            // Assert
            result.IsSuccess.Should().BeTrue();
            Mock.Get(wrapped.Object).Verify(w => w.ActivateRuleAsync(It.IsAny<Rule>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateRuleAsync_CallsWrappedEngineAndReturnsResult()
        {
            // Arrange
            var wrapped = new Mock<IRulesEngine>();

            var genericRule = Rule.Create<RulesetNames, ConditionNames>("r2")
                .InRuleset(RulesetNames.Type1)
                .SetContent(new object())
                .Since(DateTime.UtcNow)
                .Build()
                .Rule;

            Mock.Get(wrapped.Object)
                .Setup(w => w.DeactivateRuleAsync(It.IsAny<Rule>()))
                .ReturnsAsync(Operation.Success())
                .Verifiable();

            var sut = new RulesEngine<RulesetNames, ConditionNames>(wrapped.Object);

            // Act
            var result = await sut.DeactivateRuleAsync(genericRule);

            // Assert
            result.IsSuccess.Should().BeTrue();
            Mock.Get(wrapped.Object).Verify(w => w.DeactivateRuleAsync(It.IsAny<Rule>()), Times.Once);
        }

        [Fact]
        public async Task AddRuleAsync_CallsWrappedEngineAndReturnsResult()
        {
            // Arrange
            var wrapped = new Mock<IRulesEngine>();

            var genericRule = Rule.Create<RulesetNames, ConditionNames>("r3")
                .InRuleset(RulesetNames.Type1)
                .SetContent(new object())
                .Since(DateTime.UtcNow)
                .Build()
                .Rule;

            var option = RuleAddPriorityOption.AtLargestNumber;

            Mock.Get(wrapped.Object)
                .Setup(w => w.AddRuleAsync(It.IsAny<Rule>(), It.IsAny<RuleAddPriorityOption>()))
                .ReturnsAsync(Operation.Success())
                .Verifiable();

            var sut = new RulesEngine<RulesetNames, ConditionNames>(wrapped.Object);

            // Act
            var result = await sut.AddRuleAsync(genericRule, option);

            // Assert
            result.IsSuccess.Should().BeTrue();
            Mock.Get(wrapped.Object).Verify(w => w.AddRuleAsync(It.IsAny<Rule>(), It.IsAny<RuleAddPriorityOption>()), Times.Once);
        }

        [Fact]
        public async Task UpdateRuleAsync_CallsWrappedEngineAndReturnsResult()
        {
            // Arrange
            var wrapped = new Mock<IRulesEngine>();

            var genericRule = Rule.Create<RulesetNames, ConditionNames>("r4")
                .InRuleset(RulesetNames.Type1)
                .SetContent(new object())
                .Since(DateTime.UtcNow)
                .Build()
                .Rule;

            Mock.Get(wrapped.Object)
                .Setup(w => w.UpdateRuleAsync(It.IsAny<Rule>()))
                .ReturnsAsync(Operation.Success())
                .Verifiable();

            var sut = new RulesEngine<RulesetNames, ConditionNames>(wrapped.Object);

            // Act
            var result = await sut.UpdateRuleAsync(genericRule);

            // Assert
            result.IsSuccess.Should().BeTrue();
            Mock.Get(wrapped.Object).Verify(w => w.UpdateRuleAsync(It.IsAny<Rule>()), Times.Once);
        }

        [Fact]
        public async Task CreateRulesetAsync_CallsWrappedEngine()
        {
            // Arrange
            var wrapped = new Mock<IRulesEngine>();
            var sut = new RulesEngine<RulesetNames, ConditionNames>(wrapped.Object);

            wrapped.Setup(w => w.CreateRulesetAsync(It.IsAny<string>())).ReturnsAsync(Operation.Success()).Verifiable();

            // Act
            await sut.CreateRulesetAsync(RulesetNames.Type1);

            // Assert
            wrapped.Verify(w => w.CreateRulesetAsync(It.Is<string>(s => s == GenericConversions.Convert(RulesetNames.Type1))), Times.Once);
        }

        [Fact]
        public async Task MatchManyAsync_ConvertsConditionsAndReturnsGenericRules()
        {
            // Arrange
            var wrapped = new Mock<IRulesEngine>();

            var nonGenericRule = Rule.Create("rA").InRuleset(RulesetNames.Type1.ToString()).SetContent(new object()).Since(DateTime.UtcNow).Build().Rule;
            nonGenericRule.Priority = 1;

            wrapped.Setup(w => w.MatchManyAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<IDictionary<string, object>>()))
                .ReturnsAsync([nonGenericRule]);

            var sut = new RulesEngine<RulesetNames, ConditionNames>(wrapped.Object);

            var conditions = new Dictionary<ConditionNames, object>
            {
                { ConditionNames.IsoCountryCode, "PT" }
            };

            // Act
            var result = await sut.MatchManyAsync(RulesetNames.Type1, DateTime.UtcNow, conditions);

            // Assert
            result.Should().NotBeNull().And.HaveCount(1);
            result.First().Should().BeOfType<Rule<RulesetNames, ConditionNames>>();
            Mock.Get(wrapped.Object).Verify(w => w.MatchManyAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
        }

        [Fact]
        public async Task MatchOneAsync_ConvertsConditionsAndReturnsGenericRule()
        {
            // Arrange
            var wrapped = new Mock<IRulesEngine>();

            var nonGenericRule = Rule.Create("rB").InRuleset(RulesetNames.Type1.ToString()).SetContent(new object()).Since(DateTime.UtcNow).Build().Rule;
            nonGenericRule.Priority = 2;

            wrapped.Setup(w => w.MatchOneAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<IDictionary<string, object>>()))
                .ReturnsAsync(nonGenericRule);

            var sut = new RulesEngine<RulesetNames, ConditionNames>(wrapped.Object);

            var conditions = new Dictionary<ConditionNames, object>
            {
                { ConditionNames.IsoCurrency, "EUR" }
            };

            // Act
            var result = await sut.MatchOneAsync(RulesetNames.Type1, DateTime.UtcNow, conditions);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Rule<RulesetNames, ConditionNames>>();
            Mock.Get(wrapped.Object).Verify(w => w.MatchOneAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_WithConditions_ConvertsAndReturnsGenericRules()
        {
            // Arrange
            var wrapped = new Mock<IRulesEngine>();

            var nonGenericRule = Rule.Create("rC").InRuleset(RulesetNames.Type1.ToString()).SetContent(new object()).Since(DateTime.UtcNow).Build().Rule;
            nonGenericRule.Priority = 3;

            wrapped.Setup(w => w.SearchAsync(It.IsAny<SearchArgs<string, string>>()))
                .ReturnsAsync([nonGenericRule]);

            var sut = new RulesEngine<RulesetNames, ConditionNames>(wrapped.Object);

            var searchArgs = new SearchArgs<RulesetNames, ConditionNames>(RulesetNames.Type1, DateTime.MinValue, DateTime.MaxValue)
            {
                Conditions = new Dictionary<ConditionNames, object>
                {
                    { ConditionNames.IsoCountryCode, "PT" }
                }
            };

            // Act
            var result = await sut.SearchAsync(searchArgs);

            // Assert
            result.Should().NotBeNull().And.HaveCount(1);
            result.First().Should().BeOfType<Rule<RulesetNames, ConditionNames>>();
            Mock.Get(wrapped.Object).Verify(w => w.SearchAsync(It.IsAny<SearchArgs<string, string>>()), Times.Once);
        }
    }
}
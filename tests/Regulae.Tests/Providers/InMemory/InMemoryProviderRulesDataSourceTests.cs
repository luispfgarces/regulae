namespace Regulae.Tests.Providers.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Providers.InMemory;
    using Regulae.Providers.InMemory.DataModel;
    using Xunit;

    public class InMemoryProviderRulesDataSourceTests
    {
        public static IEnumerable<object[]> CtorArguments { get; } =
        [
            [null, null],
            [Mock.Of<IInMemoryRulesStorage>(), null]
        ];

        [Fact]
        public async Task AddRuleAsync_GivenNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Rule rule = null;

            var inMemoryRulesStorage = Mock.Of<IInMemoryRulesStorage>();
            var ruleFactory = Mock.Of<IRuleFactory>();

            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(rule))
                .Verifiable();

            Mock.Get(inMemoryRulesStorage)
                .Setup(x => x.AddRule(It.IsAny<RuleDataModel>()))
                .Verifiable();

            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory);

            // Act
            var argumentNullException = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await inMemoryProviderRulesDataSource.AddRuleAsync(rule));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(rule));
        }

        [Fact]
        public async Task AddRuleAsync_GivenRule_ConvertsToRuleDataModelAndAddsToDataSource()
        {
            // Arrange
            var rule = new Rule("Test rule", "Test ruleset", DateTime.UtcNow, null, new ObjectContentContainer(new object()));
            var ruleDataModel = new RuleDataModel
            {
                Content = new object(),
                DateBegin = DateTime.UtcNow,
                Name = "Test",
                Ruleset = "TestRuleset",
            };

            var inMemoryRulesStorage = Mock.Of<IInMemoryRulesStorage>();
            var ruleFactory = Mock.Of<IRuleFactory>();

            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(rule))
                .Returns(ruleDataModel)
                .Verifiable();

            Mock.Get(inMemoryRulesStorage)
                .Setup(x => x.AddRule(ruleDataModel))
                .Verifiable();

            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory);

            // Act
            await inMemoryProviderRulesDataSource.AddRuleAsync(rule);

            // Assert
            Mock.VerifyAll(Mock.Get(inMemoryRulesStorage), Mock.Get(ruleFactory));
        }

        [Theory]
        [MemberData(nameof(CtorArguments))]
        public void Ctor_GivenNullParameters_ThrowsArgumentNullException(object param1, object param2)
        {
            // Arrange
            var inMemoryRulesStorage = param1 as IInMemoryRulesStorage;
            var ruleFactory = param2 as IRuleFactory;

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory));

            //Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().BeOneOf(nameof(inMemoryRulesStorage), nameof(ruleFactory));
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Rule rule = null;

            var inMemoryRulesStorage = Mock.Of<IInMemoryRulesStorage>();
            var ruleFactory = Mock.Of<IRuleFactory>();

            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(rule))
                .Verifiable();

            Mock.Get(inMemoryRulesStorage)
                .Setup(x => x.UpdateRule(It.IsAny<RuleDataModel>()))
                .Verifiable();

            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory);

            // Act
            var argumentNullException = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await inMemoryProviderRulesDataSource.UpdateRuleAsync(rule));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(rule));
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenRule_ConvertsToRuleDataModelAndUpdatesOnDataSource()
        {
            // Arrange
            var rule = new Rule("Test rule", "Test ruleset", DateTime.UtcNow, null, new ObjectContentContainer(new object()));
            var ruleDataModel = new RuleDataModel
            {
                Content = new object(),
                DateBegin = DateTime.UtcNow,
                Name = "Test",
                Ruleset = "TestRuleset",
            };

            var inMemoryRulesStorage = Mock.Of<IInMemoryRulesStorage>();
            var ruleFactory = Mock.Of<IRuleFactory>();

            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(rule))
                .Returns(ruleDataModel)
                .Verifiable();

            Mock.Get(inMemoryRulesStorage)
                .Setup(x => x.UpdateRule(ruleDataModel))
                .Verifiable();

            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory);

            // Act
            await inMemoryProviderRulesDataSource.UpdateRuleAsync(rule);

            // Assert
            Mock.VerifyAll(Mock.Get(inMemoryRulesStorage), Mock.Get(ruleFactory));
        }

        [Fact]
        public async Task CreateConditionAsync_ValidatesName_AndCreates()
        {
            // Arrange
            var storage = new Mock<IInMemoryRulesStorage>();
            storage.Setup(s => s.CreateCondition("c1", DataTypes.Boolean)).Verifiable();

            var factory = Mock.Of<IRuleFactory>();
            var ds = new InMemoryProviderRulesDataSource(storage.Object, factory);

            // Act
            await ds.CreateConditionAsync("c1", DataTypes.Boolean);

            // Assert
            storage.Verify();
        }

        [Fact]
        public async Task CreateConditionAsync_NullOrWhitespace_Throws()
        {
            // Arrange
            var storage = Mock.Of<IInMemoryRulesStorage>();
            var factory = Mock.Of<IRuleFactory>();
            var ds = new InMemoryProviderRulesDataSource(storage, factory);

            // Act
            Func<Task> a1 = async () => await ds.CreateConditionAsync(null!, DataTypes.Integer);
            Func<Task> a2 = async () => await ds.CreateConditionAsync(" ", DataTypes.Integer);

            // Assert
            var e1 = await a1.Should().ThrowAsync<ArgumentNullException>();
            e1.Which.ParamName.Should().Be("name");
            var e2 = await a2.Should().ThrowAsync<ArgumentNullException>();
            e2.Which.ParamName.Should().Be("name");
        }

        [Fact]
        public async Task CreateRulesetAsync_Creates_WhenNotExists_ThrowsWhenExists()
        {
            // Arrange 1
            var storage = new Mock<IInMemoryRulesStorage>();
            storage.Setup(s => s.GetRulesets()).Returns([]);
            storage.Setup(s => s.CreateRuleset("rsx")).Verifiable();

            var ds = new InMemoryProviderRulesDataSource(storage.Object, Mock.Of<IRuleFactory>());

            // Act 1
            await ds.CreateRulesetAsync("rsx");

            // Assert 1
            storage.Verify(s => s.CreateRuleset("rsx"), Times.Once);

            // Arrange 2
            storage.Setup(s => s.GetRulesets()).Returns([new RulesetDataModel { Name = "rsx", Creation = DateTime.UtcNow, Rules = [] }]);

            // Act 2
            Func<Task> act = async () => await ds.CreateRulesetAsync("rsx");

            // Assert 2
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateRulesetAsync_Creates_WhenNullName_ThrowsArgumentNullException()
        {
            // Arrange
            var ds = new InMemoryProviderRulesDataSource(Mock.Of<IInMemoryRulesStorage>(), Mock.Of<IRuleFactory>());

            // Act
            Func<Task> act = async () => await ds.CreateRulesetAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("name");
        }

        [Fact]
        public async Task GetRulesAsync_FiltersByDateAndRuleset()
        {
            // Arrange
            var ruleset = "rsf";
            var storage = new Mock<IInMemoryRulesStorage>();

            var now = DateTime.UtcNow;
            var r1 = new RuleDataModel { Name = "r1", Ruleset = ruleset, DateBegin = now.AddDays(-10), DateEnd = now.AddDays(-5), Priority = 1, Content = new object() };
            var r2 = new RuleDataModel { Name = "r2", Ruleset = ruleset, DateBegin = now.AddDays(-1), DateEnd = now.AddDays(5), Priority = 2, Content = new object() };
            var r3 = new RuleDataModel { Name = "r3", Ruleset = ruleset, DateBegin = now.AddDays(-2), DateEnd = null, Priority = 3, Content = new object() };

            storage.Setup(s => s.GetRulesBy(ruleset)).Returns([r1, r2, r3]);

            var factory = new Mock<IRuleFactory>();
            factory.Setup(f => f.CreateRule(r2)).Returns(new Rule(r2.Name, r2.Ruleset, r2.DateBegin, r2.DateEnd, new ObjectContentContainer(r2.Content)));
            factory.Setup(f => f.CreateRule(r3)).Returns(new Rule(r3.Name, r3.Ruleset, r3.DateBegin, r3.DateEnd, new ObjectContentContainer(r3.Content)));

            var ds = new InMemoryProviderRulesDataSource(storage.Object, factory.Object);

            // Act
            var results = await ds.GetRulesAsync(ruleset, now.AddDays(-3), now.AddDays(1));

            // Assert
            results.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetRulesByAsync_FiltersByArgs()
        {
            // Arrange
            var storage = new Mock<IInMemoryRulesStorage>();
            var all = new List<RuleDataModel>
            {
                new() { Name = "A", Ruleset = "rs1", Content = new object(), DateBegin = DateTime.UtcNow },
                new() { Name = "B", Ruleset = "rs2", Content = new object(), DateBegin = DateTime.UtcNow },
                new() { Name = "C", Ruleset = "rs1", Content = new object(), DateBegin = DateTime.UtcNow },
            };
            storage.Setup(s => s.GetAllRules()).Returns(all);

            var factory = new Mock<IRuleFactory>();
            factory.Setup(f => f.CreateRule(It.IsAny<RuleDataModel>())).Returns((RuleDataModel r) => new Rule(r.Name, r.Ruleset, r.DateBegin, r.DateEnd, new ObjectContentContainer(r.Content)));

            var ds = new InMemoryProviderRulesDataSource(storage.Object, factory.Object);

            // Act 1
            var filtered = await ds.GetRulesByAsync(new RulesFilterArgs { Ruleset = "rs1" });

            // Assert 1
            filtered.Should().HaveCount(2);

            // Act 2
            var filteredName = await ds.GetRulesByAsync(new RulesFilterArgs { Name = "B" });

            // Assert 2
            filteredName.Should().HaveCount(1);

            // Act 3
            var filteredPriority = await ds.GetRulesByAsync(new RulesFilterArgs { Priority = 0 });

            // Assert 3
            // none should match exact priority 0
            filteredPriority.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRulesByAsync_NullArgs_ThrowsArgumentNullException()
        {
            // Arrange
            var storage = new Mock<IInMemoryRulesStorage>();
            var factory = new Mock<IRuleFactory>();
            var ds = new InMemoryProviderRulesDataSource(storage.Object, factory.Object);

            // Act 1
            var act = async () => await ds.GetRulesByAsync(null);

            // Assert 1
            await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("rulesFilterArgs");
        }

        [Fact]
        public async Task GetRulesetsAsync_ReturnsRulesets()
        {
            // Arrange
            var storage = new Mock<IInMemoryRulesStorage>();
            var rs = new List<RulesetDataModel> { new() { Name = "r1", Creation = DateTime.UtcNow, Rules = [] } };
            storage.Setup(s => s.GetRulesets()).Returns(rs);

            var ds = new InMemoryProviderRulesDataSource(storage.Object, Mock.Of<IRuleFactory>());

            // Act
            var res = await ds.GetRulesetsAsync();

            // Assert
            res.Should().ContainKey("r1");
        }

        [Fact]
        public async Task GetConditionsAsync_ReturnsConvertedDictionary()
        {
            // Arrange
            var storage = new Mock<IInMemoryRulesStorage>();
            var dict = new Dictionary<string, ConditionDataModel>
            {
                { "c1", new ConditionDataModel { Name = "c1", DataType = DataTypes.Boolean, Creation = DateTime.UtcNow } }
            };
            storage.Setup(s => s.GetConditions()).Returns(dict);

            var ds = new InMemoryProviderRulesDataSource(storage.Object, Mock.Of<IRuleFactory>());

            // Act
            var res = await ds.GetConditionsAsync();

            // Assert
            res.Should().ContainKey("c1");
            res["c1"].DataType.Should().Be(DataTypes.Boolean);
        }
    }
}
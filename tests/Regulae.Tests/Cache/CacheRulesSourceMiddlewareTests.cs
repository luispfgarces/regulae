namespace Regulae.Tests.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Cache;
    using Regulae.Source;
    using Xunit;

    public class CacheRulesSourceMiddlewareTests
    {
        private static CacheRulesSourceMiddleware CreateMiddleware(Mock<ICache> cacheMock = null)
            => new CacheRulesSourceMiddleware(cacheMock?.Object ?? new Mock<ICache>().Object);

        [Fact]
        public async Task HandleAddRuleAsync_ShouldEvictRulesAllAndRulesetPrefix()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var args = new AddRuleArgs { Rule = new Rule("r", "ruleset1", DateTime.Now, null, Mock.Of<IContentContainer>()) };

            cache.Setup(c => c.Evict("Regulae:Rules:All"));
            cache.Setup(c => c.EvictMany("Regulae:Rules:ruleset1"));

            var middleware = CreateMiddleware(cache);
            var called = false;
            await middleware.HandleAddRuleAsync(args, a =>
            {
                called = true;
                return ValueTask.CompletedTask;
            });

            called.Should().BeTrue();
            cache.VerifyAll();
        }

        [Fact]
        public async Task HandleCreateConditionAsync_ShouldEvictConditionsAll()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var args = new CreateConditionArgs { DataType = DataTypes.Boolean, Name = "cond" };

            cache.Setup(c => c.Evict("Regulae:Conditions:All"));

            var middleware = CreateMiddleware(cache);
            var called = false;
            await middleware.HandleCreateConditionAsync(args, a =>
            {
                called = true;
                return ValueTask.CompletedTask;
            });

            called.Should().BeTrue();
            cache.VerifyAll();
        }

        [Fact]
        public async Task HandleCreateRulesetAsync_ShouldEvictRulesetsAll()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var args = new CreateRulesetArgs { Name = "ruleset1" };

            cache.Setup(c => c.Evict("Regulae:Rulesets:All"));

            var middleware = CreateMiddleware(cache);
            var called = false;
            await middleware.HandleCreateRulesetAsync(args, a =>
            {
                called = true;
                return ValueTask.CompletedTask;
            });

            called.Should().BeTrue();
            cache.VerifyAll();
        }

        [Fact]
        public async Task HandleGetConditionsAsync_ShouldReturnFromCache_IfPresent()
        {
            var cache = new Mock<ICache>();
            var expected = new Dictionary<string, Condition>();
            cache.Setup(c => c.TryGet("Regulae:Conditions:All", out It.Ref<object>.IsAny))
                .Callback(new TryGetCallback((string key, out object value) =>
                {
                    value = expected;
                }))
                .Returns(true);

            var middleware = CreateMiddleware(cache);
            var args = new GetConditionsArgs();
            var result = await middleware.HandleGetConditionsAsync(args, _ => throw new Exception("Should not call next"));

            result.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task HandleGetConditionsAsync_ShouldCallNextAndCache_IfNotPresent()
        {
            var cache = new Mock<ICache>();
            object dummy;
            cache.Setup(c => c.TryGet("Regulae:Conditions:All", out dummy)).Returns(false);
            var expected = new Dictionary<string, Condition>();
            cache.Setup(c => c.Set("Regulae:Conditions:All", expected)).Returns(expected);

            var middleware = CreateMiddleware(cache);
            var args = new GetConditionsArgs();
            var result = await middleware.HandleGetConditionsAsync(args, _ => ValueTask.FromResult((IReadOnlyDictionary<string, Condition>)expected));

            result.Should().BeSameAs(expected);
            cache.Verify(c => c.Set("Regulae:Conditions:All", expected), Times.Once);
        }

        [Fact]
        public async Task HandleGetRulesAsync_ShouldReturnFromCache_IfPresent()
        {
            var cache = new Mock<ICache>();
            var rules = new List<Rule> { new Rule("n", "r", DateTime.Now, null, Mock.Of<IContentContainer>()) };
            var args = new GetRulesArgs { Ruleset = "r", DateBegin = new DateTime(2020, 1, 2), DateEnd = new DateTime(2020, 2, 3) };
            var cacheKey = $"Regulae:Rules:{{0}}r_202012_202023";

            cache.Setup(c => c.TryGet(cacheKey, out It.Ref<object>.IsAny))
                .Callback(new TryGetCallback((string k, out object v) => { v = rules; }))
                .Returns(true);

            var middleware = CreateMiddleware(cache);
            var result = await middleware.HandleGetRulesAsync(args, _ => throw new Exception("Should not call next"));

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task HandleGetRulesAsync_ShouldCallNextAndCache_IfNotPresent()
        {
            var cache = new Mock<ICache>();
            var rules = new List<Rule> { new Rule("n", "r", DateTime.Now, null, Mock.Of<IContentContainer>()) };
            var args = new GetRulesArgs { Ruleset = "r", DateBegin = new DateTime(2020, 1, 2), DateEnd = new DateTime(2020, 2, 3) };
            var cacheKey = $"Regulae:Rules:{{0}}r_202012_202023";

            object dummy;
            cache.Setup(c => c.TryGet(cacheKey, out dummy)).Returns(false);
            cache.Setup(c => c.Set(cacheKey, rules)).Returns(rules);

            var middleware = CreateMiddleware(cache);
            var result = await middleware.HandleGetRulesAsync(args, _ => ValueTask.FromResult((IReadOnlyCollection<Rule>)rules));

            result.Should().BeSameAs(rules);
            cache.Verify(c => c.Set(cacheKey, rules), Times.Once);
        }

        [Fact]
        public async Task HandleGetRulesetsAsync_ShouldReturnFromCache_IfPresent()
        {
            var cache = new Mock<ICache>();
            var expected = new Dictionary<string, Ruleset>();
            cache.Setup(c => c.TryGet("Regulae:Rulesets:All", out It.Ref<object>.IsAny))
                .Callback(new TryGetCallback((string key, out object value) =>
                {
                    value = expected;
                }))
                .Returns(true);

            var middleware = CreateMiddleware(cache);
            var args = new GetRulesetsArgs();
            var result = await middleware.HandleGetRulesetsAsync(args, _ => throw new Exception("Should not call next"));

            result.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task HandleGetRulesetsAsync_ShouldCallNextAndCache_IfNotPresent()
        {
            var cache = new Mock<ICache>();
            object dummy;
            cache.Setup(c => c.TryGet("Regulae:Rulesets:All", out dummy)).Returns(false);
            var expected = new Dictionary<string, Ruleset>();
            cache.Setup(c => c.Set("Regulae:Rulesets:All", expected)).Returns(expected);

            var middleware = CreateMiddleware(cache);
            var args = new GetRulesetsArgs();
            var result = await middleware.HandleGetRulesetsAsync(args, _ => ValueTask.FromResult((IReadOnlyDictionary<string, Ruleset>)expected));

            result.Should().BeSameAs(expected);
            cache.Verify(c => c.Set("Regulae:Rulesets:All", expected), Times.Once);
        }

        [Fact]
        public async Task HandleGetRulesFilteredAsync_ShouldCallNextOnly()
        {
            var middleware = CreateMiddleware();
            var args = new GetRulesFilteredArgs();
            var expected = new List<Rule> { new Rule("n", "r", DateTime.Now, null, Mock.Of<IContentContainer>()) };

            var result = await middleware.HandleGetRulesFilteredAsync(args, _ => ValueTask.FromResult((IReadOnlyCollection<Rule>)expected));

            result.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task HandleUpdateRuleAsync_ShouldEvictRulesAllAndRulesetPrefix()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var args = new UpdateRuleArgs { Rule = new Rule("r", "ruleset1", DateTime.Now, null, Mock.Of<IContentContainer>()) };

            cache.Setup(c => c.Evict("Regulae:Rules:All"));
            cache.Setup(c => c.EvictMany("Regulae:Rules:ruleset1"));

            var middleware = CreateMiddleware(cache);
            var called = false;
            await middleware.HandleUpdateRuleAsync(args, a =>
            {
                called = true;
                return ValueTask.CompletedTask;
            });

            called.Should().BeTrue();
            cache.VerifyAll();
        }

        private delegate void TryGetCallback(string key, out object value);
    }
}
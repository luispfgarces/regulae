namespace Regulae.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Regulae;
    using Regulae.Providers.MongoDb.DataModel;

    internal sealed class MongoDbProviderRulesDataSource : IRulesDataSource
    {
        private readonly IMongoDatabase mongoDatabase;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;
        private readonly IRuleFactory ruleFactory;

        public MongoDbProviderRulesDataSource(
            IMongoClient mongoClient,
            MongoDbProviderSettings mongoDbProviderSettings,
            IRuleFactory ruleFactory)
        {
            if (mongoClient is null)
            {
                throw new ArgumentNullException(nameof(mongoClient));
            }

            this.mongoDbProviderSettings = mongoDbProviderSettings ?? throw new ArgumentNullException(nameof(mongoDbProviderSettings));
            this.ruleFactory = ruleFactory ?? throw new ArgumentNullException(nameof(ruleFactory));
            this.mongoDatabase = mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
        }

        public async ValueTask AddRuleAsync(Rule rule)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            await rulesCollection.InsertOneAsync(ruleDataModel).ConfigureAwait(false);
        }

        public async ValueTask CreateConditionAsync(string name, DataTypes dataType)
        {
            var conditionsCollection = this.mongoDatabase.GetCollection<ConditionDataModel>(this.mongoDbProviderSettings.ConditionsCollectionName);

            var conditionDataModel = new ConditionDataModel
            {
                Creation = DateTime.UtcNow,
                DataType = dataType.ToString(),
                Id = Guid.NewGuid(),
                Name = name,
            };

            await conditionsCollection.InsertOneAsync(conditionDataModel).ConfigureAwait(false);
        }

        public async ValueTask CreateRulesetAsync(string ruleset)
        {
            var rulesetsCollection = this.mongoDatabase.GetCollection<RulesetDataModel>(this.mongoDbProviderSettings.RulesetsCollectionName);

            var rulesetDataModel = new RulesetDataModel
            {
                Creation = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Name = ruleset,
            };

            await rulesetsCollection.InsertOneAsync(rulesetDataModel).ConfigureAwait(false);
        }

        public async ValueTask<IReadOnlyDictionary<string, Condition>> GetConditionsAsync()
        {
            var conditionsCollection = this.mongoDatabase.GetCollection<ConditionDataModel>(this.mongoDbProviderSettings.ConditionsCollectionName);

            var findAllFilterDefinition = FilterDefinition<ConditionDataModel>.Empty;

            var resultsCursor = await conditionsCollection.FindAsync(findAllFilterDefinition).ConfigureAwait(false);
            var conditions = new Dictionary<string, Condition>(StringComparer.Ordinal);
            while (await resultsCursor.MoveNextAsync().ConfigureAwait(false))
            {
                foreach (var conditionDataModel in resultsCursor.Current)
                {
                    conditions.Add(conditionDataModel.Name, new Condition(conditionDataModel.Name, conditionDataModel.Creation, Enum.Parse<DataTypes>(conditionDataModel.DataType)));
                }
            }

            return conditions;
        }

        public async ValueTask<IReadOnlyCollection<Rule>> GetRulesAsync(string ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            var getRulesByRulesetAndDatesInterval =
                BuildFilterByRulesetAndDatesInterval(ruleset, dateBegin, dateEnd);

            return await this.GetRulesAsync(getRulesByRulesetAndDatesInterval).ConfigureAwait(false);
        }

        public ValueTask<IReadOnlyCollection<Rule>> GetRulesByAsync(RulesFilterArgs rulesFilterArgs)
        {
            if (rulesFilterArgs is null)
            {
                throw new ArgumentNullException(nameof(rulesFilterArgs));
            }

            var filterDefinition =
                BuildFilterFromRulesFilterArgs(rulesFilterArgs);

            return this.GetRulesAsync(filterDefinition);
        }

        public async ValueTask<IReadOnlyDictionary<string, Ruleset>> GetRulesetsAsync()
        {
            var rulesetsCollection = this.mongoDatabase.GetCollection<RulesetDataModel>(this.mongoDbProviderSettings.RulesetsCollectionName);

            var findAllFilterDefinition = FilterDefinition<RulesetDataModel>.Empty;

            var resultsCursor = await rulesetsCollection.FindAsync(findAllFilterDefinition).ConfigureAwait(false);
            var rulesets = new Dictionary<string, Ruleset>(StringComparer.Ordinal);
            while (await resultsCursor.MoveNextAsync().ConfigureAwait(false))
            {
                foreach (var rulesetDataModel in resultsCursor.Current)
                {
                    rulesets.Add(rulesetDataModel.Name, new Ruleset(rulesetDataModel.Name, rulesetDataModel.Creation));
                }
            }

            return rulesets;
        }

        public async ValueTask UpdateRuleAsync(Rule rule)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            var filterDefinition = Builders<RuleDataModel>.Filter.Eq(x => x.Name, ruleDataModel.Name);
            FieldDefinition<RuleDataModel, object> contentField = "Content";
            var updateDefinitions = new UpdateDefinition<RuleDataModel>[]
            {
                Builders<RuleDataModel>.Update.Set(contentField, (object)ruleDataModel.Content),
                Builders<RuleDataModel>.Update.Set(r => r.Ruleset, ruleDataModel.Ruleset),
                Builders<RuleDataModel>.Update.Set(r => r.DateBegin, ruleDataModel.DateBegin),
                Builders<RuleDataModel>.Update.Set(r => r.DateEnd, ruleDataModel.DateEnd),
                Builders<RuleDataModel>.Update.Set(r => r.Name, ruleDataModel.Name),
                Builders<RuleDataModel>.Update.Set(r => r.Priority, ruleDataModel.Priority),
                Builders<RuleDataModel>.Update.Set(r => r.Active, ruleDataModel.Active),
                Builders<RuleDataModel>.Update.Set(r => r.RootCondition, ruleDataModel.RootCondition),
            };

            var updateDefinition = Builders<RuleDataModel>.Update.Combine(updateDefinitions);

            await rulesCollection.UpdateOneAsync(filterDefinition, updateDefinition).ConfigureAwait(false);
        }

        private static FilterDefinition<RuleDataModel> BuildFilterByRulesetAndDatesInterval(string ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            var rulesetFilter = Builders<RuleDataModel>.Filter.Eq(x => x.Ruleset, ruleset);

            var datesFilter = Builders<RuleDataModel>.Filter.And(
                Builders<RuleDataModel>.Filter.Lte(rule => rule.DateBegin, dateEnd),
                Builders<RuleDataModel>.Filter.Or(
                    Builders<RuleDataModel>.Filter.Gt(rule => rule.DateEnd, dateBegin),
                    Builders<RuleDataModel>.Filter.Eq(rule => rule.DateEnd, null))
                );

            return Builders<RuleDataModel>.Filter.And(rulesetFilter, datesFilter);
        }

        private static FilterDefinition<RuleDataModel> BuildFilterFromRulesFilterArgs(RulesFilterArgs rulesFilterArgs)
        {
            var filtersToApply = new List<FilterDefinition<RuleDataModel>>(3);

            if (!Equals(rulesFilterArgs.Ruleset, default(string)))
            {
                filtersToApply.Add(Builders<RuleDataModel>.Filter.Eq(x => x.Ruleset, rulesFilterArgs.Ruleset));
            }

            if (!string.IsNullOrWhiteSpace(rulesFilterArgs.Name))
            {
                filtersToApply.Add(Builders<RuleDataModel>.Filter.Eq(x => x.Name, rulesFilterArgs.Name));
            }

            if (rulesFilterArgs.Priority.HasValue)
            {
                filtersToApply.Add(Builders<RuleDataModel>.Filter.Eq(x => x.Priority, rulesFilterArgs.Priority.GetValueOrDefault()));
            }

            return filtersToApply.Any() ? Builders<RuleDataModel>.Filter.And(filtersToApply) : Builders<RuleDataModel>.Filter.Empty;
        }

        private async ValueTask<IReadOnlyCollection<Rule>> GetRulesAsync(FilterDefinition<RuleDataModel> getRulesByRulesetAndDatesInterval)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var filterOptions = new FindOptions<RuleDataModel>
            {
                Sort = Builders<RuleDataModel>.Sort.Ascending(x => x.Priority),
            };
            var fetchedRulesCursor = await rulesCollection.FindAsync(getRulesByRulesetAndDatesInterval, filterOptions).ConfigureAwait(false);

            var fetchedRules = await fetchedRulesCursor.ToListAsync().ConfigureAwait(false);

            // We won't use LINQ from this point onwards to avoid projected queries to database at a
            // later point. This approach assures the definitive realization of the query results
            // and does not produce side effects later on.
            var result = new Rule[fetchedRules.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = this.ruleFactory.CreateRule(fetchedRules[i]);
            }

            return result;
        }
    }
}
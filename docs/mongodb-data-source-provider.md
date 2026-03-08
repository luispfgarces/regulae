# MongoDB Data Source Provider

The MongoDB data source provider enables persistent storage of rules in a MongoDB database. It leverages the MongoDB C# driver's BSON deserialization capabilities for flexible content handling.

## Key Characteristics

- **Persistent Storage**: Rules survive application restarts and can be shared across instances.
- **Flexible Content**: Supports complex objects via BSON serialization.
- **Scalable**: Suitable for production environments with high rule volumes.
- **ACID Compliance**: Benefits from MongoDB's transactional guarantees.

## Basic Setup

For simple use cases:

```csharp
var mongoClient = new MongoClient("mongodb://localhost:27017");
var settings = new MongoDbProviderSettings
{
    DatabaseName = "rules-db",
    RulesCollectionName = "rules"
};

var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .SetMongoDbDataSource(mongoClient, settings)
    .Build();
```

### Dependency Injection with Service Provider

For DI-enabled applications:

```csharp
// In DI setup
services.AddMongoDbRulesDataSource(
    mongoClient,
    settings,
    ServiceLifetime.Singleton);

// When creating the engine
var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .WithMongoDbDataSource(serviceProvider)
    .Build();
```

## Content Object Guidelines

When defining rule content for MongoDB storage:

- **Use Simple Properties**: Define get/set properties without behavior.
- **Parameterless Constructor**: Include a default constructor (can be protected/private).
- **Avoid Logic**: Keep content objects as pure data structures (anemic objects).

This ensures reliable serialization/deserialization and prevents loading issues.

## Usage Notes

- **Connection Management**: Handle MongoDB connection pooling and authentication appropriately.
- **Indexing**: Consider indexing on ruleset, dates, and frequently queried conditions for performance.
- **Migration**: Rules stored in MongoDB persist across deployments; plan for schema evolution (rule content).
- **Backup**: Implement regular backups for production rule data.

For development/testing, consider the [in-memory provider](in-memory-data-source-provider.md). To create custom providers, see [this guide](create-new-data-source-provider.md).

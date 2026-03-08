# In-Memory Data Source Provider

The in-memory data source provider allows you to define and manage rules entirely in memory. This is ideal for development, testing, or scenarios where rules don't need to persist across application restarts.

## Key Characteristics

- **Volatile Storage**: Rules exist only in memory and are lost when the application stops.
- **Fast Access**: No disk I/O, making it suitable for high-performance scenarios.
- **Simple Setup**: No external dependencies required.

## Basic Setup

For simple use cases without strong typing:

```csharp
var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .SetInMemoryDataSource()
    .Build();
```

### Dependency Injection with Service Provider

For applications using dependency injection:

```csharp
// In your DI setup
services.AddInMemoryRulesDataSource(ServiceLifetime.Singleton);

// When creating the engine
var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .WithInMemoryDataSource(serviceProvider)
    .Build();
```

## Usage Notes

- **Rule Persistence**: Since data is in-memory, you'll need to recreate and add rules on each application start.
- **Thread Safety**: The in-memory provider is thread-safe for concurrent access.
- **Performance**: Best suited for applications with moderate rule counts and high evaluation frequency.

For production use with persistent storage, consider the [MongoDB provider](mongodb-data-source-provider.md) or create a [custom provider](create-new-data-source-provider.md).

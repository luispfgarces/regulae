# Regulae

![Regulae logo](https://raw.githubusercontent.com/luispfgarces/regulae/master/docs/logos/regulae-logo-565x128.png)

Regulae is a general purpose library that allows defining, evaluating, and managing rules for complex business scenarios. 

[![.NET build](https://github.com/luispfgarces/regulae/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/luispfgarces/regulae/actions/workflows/dotnet-build.yml)

## What is a rule

A rule is a data structure limited in time (`date begin` and `date end`), and that is categorized by a `ruleset`. Its applicability is constrained by `conditions`, and a `priority` value is used as untie criteria when there are multiple rules applicable.

## Why use rules

By using rules, we're able to abstract a multiplicity of business scenarios through rules configurations, instead of heavy code developments. Rules enable a fast response to change and a better control of the business logic by the product owners.

## Regulae package

[![NuGet Version](https://img.shields.io/nuget/v/Regulae?style=for-the-badge&logo=nuget&label=Regulae)](https://www.nuget.org/packages/Regulae/)


The Regulae package contains the core of the rules engine. It includes an in-memory provider for the rules data source.

### Basic usage

Build the engine with the `RulesEngineBuilder`.

```csharp
var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .SetInMemoryDataSource()
    .Build();
```
Use the `Rule` fluent builder to assemble a rule.

```csharp
var ruleForPremiumFreeSample = Rule.Create("Rule for perfume sample for premium clients.")
    .InRuleset("FreeSample")
    .SetContent("SmallPerfumeSample")
    .Since(new DateTime(2020, 01, 01))
    .ApplyWhen("ClientType", Operators.Equal, "Premium")
    .Build();
```

Add a rule to the engine with the `AddRuleAsync()`.

```csharp
await rulesEngine.AddRuleAsync(ruleForPremiumFreeSample.Rule, RuleAddPriorityOption.ByPriorityNumber(1));
```

Get a matching rule by using the `MatchOneAsync()` and passing a date and conditions.

```csharp
var matchingRule = await rulesEngine.MatchOneAsync(
        "FreeSample", 
        new DateTime(2021, 12, 25), 
        new Dictionary<string, object>
        {
            { "ClientType", "Premium" }
        });
```

### Complex scenarios

For a more thorough explanation of the Regulae library and all it enables, check the [Wiki](https://github.com/luispfgarces/regulae/wiki). 

Check also the test scenarios and samples available within the source-code, to see more elaborated examples of its application.

## Regulae.Providers.MongoDb package

[![NuGet Version](https://img.shields.io/nuget/v/Regulae.Providers.MongoDb?style=for-the-badge&logo=nuget&label=Regulae.Providers.MongoDb)](https://www.nuget.org/packages/Regulae.Providers.MongoDb/)

To keep rules persisted in a MongoDB database, use the extension method in the Providers.MongoDB package to pass your MongoClient and MongoDbProviderSettings to the `RulesEngineBuilder`.

```csharp
var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .SetInMongoDBDataSource(mongoClient, mongoDbProviderSettings)
```

## Regulae.WebUI package

[![NuGet Version](https://img.shields.io/nuget/v/Regulae.WebUI?style=for-the-badge&logo=nuget&label=Regulae.WebUI)](https://www.nuget.org/packages/Regulae.WebUI/)


The WebUI package offers a way of visualizing the rules in your web service. To configure the UI, pass the rules engine as generic to the `IApplicationBuilder` extension method provided.

```csharp
builder.Services.AddControllersWithViews()
    .AddRegulaeWebUI(registrar =>
    {
        registrar
            .AddInstance(
                "Sample 1",
                (serviceProvider, name) =>
                {
                    // Logic to get rules engine
                })
            .AddInstance(
                "Sample 2",
                async (serviceProvider, name) =>
                {
                    // Async alternative also available
                });
    });

...

// Dependencies, so that Regulae Web UI works correctly.
app.UseRouting();
app.UseAntiForgery();

app.UseRegulaeWebUI();
```

Access is done via the endpoint `{host}/regulae-ui`.

[![webUISample](https://raw.githubusercontent.com/luispfgarces/regulae/master/docs/web-ui-sample.png)](https://github.com/luispfgarces/regulae/blob/master/docs/web-ui-sample.png)

## Regulae.Rql package

[![NuGet Version](https://img.shields.io/nuget/v/Regulae.Rql?style=for-the-badge&logo=nuget&label=Regulae.Rql)](https://www.nuget.org/packages/Regulae.Rql/)

The `Regulae.Rql` package provides the parser and interpreter for RQL (Rule Query Language). RQL is a powerful language for matching and searching for rules using a simple and declarative syntax, which allows for dynamic querying of rules without having to construct complex queries in C#. [6]

For example, you can match rules with RQL like this:

```rql
// Match a single rule for a given ruleset, date, and condition
MATCH ONE RULE FOR "FreeSample" ON $2021-12-25Z$ WHEN { @ClientType is "Premium" };
```

Consider the `rulesEngine` constructed in the previous section, you can interpret RQL as follows:

```csharp
var rqlEngine = rulesEngine.GetRqlEngine();
var result = await rqlEngine.ExecuteAsync("MATCH ONE RULE FOR \"FreeSample\" ON $2021-12-25Z$ WHEN { @ClientType is \"Premium\" };");
```

## Regulae.Analyzers package

[![NuGet Version](https://img.shields.io/nuget/v/Regulae.Analyzers?style=for-the-badge&logo=nuget&label=Regulae.Analyzers)](https://www.nuget.org/packages/Regulae.Analyzers/)

To improve the development workflow when writing RQL, the `Regulae.Analyzers` package provides Roslyn-based code analysis. When this package is installed, it gives you real-time feedback inside your IDE (e.g. Visual Studio) for source code written using Regulae APIs.

This includes syntax validation to catch errors as you type, significantly improving productivity and reducing runtime bugs.

## Features

The following list presents features available:
- Rules evaluation
    - Interpreted
    - Pre-compiled
- Evaluation modes
    - Match one
    - Match many
- Rules content serialization
- Rules management (Create, Read, Update)
- Data source providers
    - In-memory
    - MongoDB
- Rule Query Language
    - Type system
        - Support for objects (prototyping)
    - Match sentences
    - Search sentences
    - Language assist support
- WebUI
    - View multiple rules engine instances
    - List rulesets
    - List/search rules
        - RQL support with auto-complete using language assist
    - RQL terminal
        - Auto-complete using language assist
- Code Analysis
    - Rewrite rule builder APIs usage from Rules.Framework to Regulae (migration).

## Documentation

See the [full documentation](https://luispfgarces.github.io/regulae/) for an in-depth walthrough on Regulae usage and features.

## Contributing

Contributions are more than welcome! Submit comments, issues or pull requests, we promise to keep an eye on them :)

Head over to [CONTRIBUTING](CONTRIBUTING.md) for further details.

## License

[MIT License](LICENSE.md)

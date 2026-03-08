
# WebUI Configuration and Usage

The Regulae Web UI provides a user interface to inspect and manage your rules.

## Configuration

To use the Web UI, you need to configure it in your `Program.cs` file.

### 1. Add the Web UI services

In your `ConfigureServices` method (or in your `Program.cs` for .NET 6+), add the Regulae Web UI services:

```csharp
builder.Services.AddControllersWithViews()
    .AddRegulaeWebUI(registrar =>
    {
        registrar.AddInstance("My Ruleset", (_, _) => myRulesEngine)
            .AddInstance("Another Ruleset", async (_, _) =>
            {
                // build and return another rules engine instance
            });
    });
```

The `AddRegulaeWebUI` method allows you to register the rules engine instances that will be available in the Web UI. You can add multiple instances with different names.

### 2. Use the Web UI middleware

In your `Configure` method (or in your `Program.cs` for .NET 6+), add the Regulae Web UI middleware:

```csharp
app.UseRegulaeWebUI(opt =>
{
    opt.DocumentTitle = "My Rules";
});
```

The `UseRegulaeWebUI` method allows you to configure the Web UI options, such as the document title.

## Usage

Once you have configured the Web UI, you can access it by navigating to `/regulae-ui` in your browser.

The Web UI allows you to:

- View all registered rules engine instances.
- Inspect the rules of each ruleset.
- Match rules using a form.
- Use the RQL (Regulae Query Language) terminal to interact with the rules engine.

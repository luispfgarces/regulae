namespace Regulae.WebUI.Sample
{
    using Regulae;
    using Regulae.IntegrationTests.Common.Scenarios;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario8;
    using Regulae.Providers.InMemory;
    using Regulae.WebUI;
    using Regulae.WebUI.Sample.Engine;
    using Regulae.WebUI.Sample.ReadmeExample;
    using Regulae.WebUI.Sample.Rules;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddRegulaeWebUI(registrar =>
                {
                    registrar.AddInstance("Readme example", (_, _) => new BasicRulesEngineExample().RulesEngine)
                        .AddInstance("Random rules example", async (_, _) =>
                        {
                            var rulesProvider = new RulesEngineProvider(new RulesBuilder(new List<IRuleSpecificationsProvider>()
                            {
                                new RulesRandomFactory()
                            }));

                            return await rulesProvider.GetRulesEngineAsync();
                        })
                        .AddInstance("Poker combinations example", async (_, _) =>
                        {
                            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                                .SetInMemoryDataSource()
                                .Build();

                            await ScenarioLoader.LoadScenarioAsync(rulesEngine, new Scenario8Data());

                            return rulesEngine;
                        });
                });

            builder.Logging.SetMinimumLevel(LogLevel.Trace).AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAntiforgery();

            app.MapControllers();

            app.UseRegulaeWebUI(opt =>
            {
                opt.DocumentTitle = "Sample rules";
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
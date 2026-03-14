namespace Regulae.WebUI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Components;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Regulae.WebUI.Services;

    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension for Regulae Web UI
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class WebUIApplicationBuilderExtensions
    {
        /// <summary>
        /// Uses the Regulae web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRegulaeWebUI(
            this IApplicationBuilder app)
        {
            return app.UseRegulaeWebUI(options => { });
        }

        /// <summary>
        /// Uses the Regulae web UI.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="webUIOptionsAction">The web UI options configuration action.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseRegulaeWebUI(
            this IApplicationBuilder app,
            Action<WebUIOptions> webUIOptionsAction)
        {
            var rulesEngineInstanceProvider = app.ApplicationServices.GetRequiredService<RulesEngineInstanceProvider>();
            rulesEngineInstanceProvider.EnumerateInstances(app.ApplicationServices);

            // Options
            var webUIOptions = new WebUIOptions();
            webUIOptionsAction(webUIOptions);
            var webUIOptionsRegistry = app.ApplicationServices.GetRequiredService<WebUIOptionsRegistry>();
            webUIOptionsRegistry.Register(webUIOptions);

            // Blazor
            var embeddedProvider = new EmbeddedFileProvider(typeof(WebUIApplicationBuilderExtensions).Assembly, "Regulae.WebUI.Assets");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = embeddedProvider,
                RequestPath = new PathString("/regulae-ui"),
                ServeUnknownFileTypes = true,
            });

            app.UseEndpoints(builder =>
            {
                builder.MapGet("/regulae-ui", ctx =>
                {
                    ctx.Response.Redirect("/regulae-ui/instance");
                    return Task.CompletedTask;
                });

                builder.MapRazorComponents<WebUIApp>()
                    .AddInteractiveServerRenderMode();
            });

            return app;
        }
    }
}
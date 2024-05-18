namespace Regulae.WebUI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.DependencyInjection;
    using Regulae.WebUI.Services;

    /// <summary>
    /// Extensions for registering Web UI under the <see cref="IMvcBuilder"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class WebUIMvcBuilderExtensions
    {
        /// <summary>
        /// Adds the Regulae web UI to the application, registering the rules engine instances
        /// available to the web UI.
        /// </summary>
        /// <param name="mvcBuilder">The MVC builder.</param>
        /// <param name="instancesRegistrationAction">The instances registration action.</param>
        /// <returns></returns>
        public static IMvcBuilder AddRegulaeWebUI(this IMvcBuilder mvcBuilder, Action<IRulesEngineInstancesRegistrar> instancesRegistrationAction)
        {
            mvcBuilder.Services.AddRegulaeWebUIServices(instancesRegistrationAction);

            return mvcBuilder
                .AddApplicationPart(typeof(WebUIMvcBuilderExtensions).Assembly);
        }

        /// <summary>
        /// Adds the Regulae web UI to the application, registering the rules engine instances
        /// available to the web UI.
        /// </summary>
        /// <param name="mvcCoreBuilder">The MVC core builder.</param>
        /// <param name="instancesRegistrationAction">The instances registration action.</param>
        /// <returns></returns>
        public static IMvcCoreBuilder AddRegulaeWebUI(this IMvcCoreBuilder mvcCoreBuilder, Action<IRulesEngineInstancesRegistrar> instancesRegistrationAction)
        {
            mvcCoreBuilder.Services.AddRegulaeWebUIServices(instancesRegistrationAction);

            return mvcCoreBuilder
                .AddApplicationPart(typeof(WebUIMvcBuilderExtensions).Assembly);
        }

        private static void AddRegulaeWebUIServices(this IServiceCollection services, Action<IRulesEngineInstancesRegistrar> instancesRegistrationAction)
        {
            var rulesEngineInstanceProvider = new RulesEngineInstanceProvider();
            instancesRegistrationAction.Invoke(rulesEngineInstanceProvider);
            services.AddSingleton(rulesEngineInstanceProvider);
            services.AddSingleton<IRulesEngineInstanceProvider>(rulesEngineInstanceProvider);
            services.AddSingleton<WebUIOptionsRegistry>();
            services.AddTransient(sp => sp.GetRequiredService<WebUIOptionsRegistry>().RegisteredOptions);

            // Blazor
            services.AddRazorComponents()
                .AddInteractiveServerComponents();
            services.AddBlazorBootstrap();
        }
    }
}
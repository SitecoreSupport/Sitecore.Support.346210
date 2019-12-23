namespace Sitecore.Support
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore;
    using Sitecore.DependencyInjection;
    using Sitecore.Support.XA.Foundation.Multisite.SiteResolvers;
    using Sitecore.XA.Foundation.Multisite.SiteResolvers;

    [UsedImplicitly]
    public class RegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IEnvironmentSitesResolver,
                    Sitecore.Support.XA.Foundation.Multisite.SiteResolvers.EnvironmentSitesResolver>();
        }
    }
}
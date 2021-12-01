using ContentHubTalk.Controllers.Api;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace ContentHubTalk.DependencyInjection
{
    public class ApiControllerConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(ContentHubAssetUsageController));
        }
    }
}
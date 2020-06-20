using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.Extensions;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;

namespace SC90.Bot
{
    public class RegisterServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISitecoreContext>(provider => new SitecoreContext());
            serviceCollection.AddTransient<ISitecoreService>(provider => new SitecoreService(Sitecore.Context.ContentDatabase));

            serviceCollection.ConfigureServices("SC90.Bot.*");
        }
    }
}
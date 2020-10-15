using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceGatewayModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetService<IConfiguration>();
            var provider = configuration.GetValue<string>("Experience:Gateway");

            if (provider.EqualsInvariant(ExperienceGateway.VirtoCommerce.ToString()))
            {
                serviceCollection.AddTransient<IProductAssociationSearchServiceGateway, ProductAssociationSearchServiceGateway>();
                serviceCollection.AddTransient<IInventorySearchServiceGateway, InventorySearchServiceGateway>();
                serviceCollection.AddTransient<IPromotionSearchServiceGateway, PromotionSearchServiceGateway>();
                serviceCollection.AddTransient<IStoreServiceGateway, StoreServiceGateway>();
                serviceCollection.AddTransient<IPricingServiceGateway, PricingServiceGateway>();
                serviceCollection.AddTransient<ITaxProviderSearchServiceGateway, TaxProviderSearchServiceGateway>();
                serviceCollection.AddTransient<IMarketingPromoServiceGateway, MarketingPromoServiceGateway>();
            }
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            //Nothing

        }

        public void Uninstall()
        {
            //Nothing
        }

    }
}


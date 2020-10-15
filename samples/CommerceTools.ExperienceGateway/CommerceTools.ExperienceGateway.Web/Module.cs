using CommerceTools.ExperienceGateway.Data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.XGateway.Core.Services;
using VirtoCommerce.XGateway.Core.Models;

namespace CommerceTools.ExperienceGateway.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetService<IConfiguration>();
            var provider = configuration.GetValue<string>("Experience:Gateway");

            if (provider.EqualsInvariant(VirtoCommerce.XGateway.Core.Models.ExperienceGateway.CommerceTools.ToString()))
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


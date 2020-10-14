using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceGatewayModule.Data.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection services)
        {
            services.AddTransient<IProductAssociationSearchServiceGateway, ProductAssociationSearchServiceGateway>();
            services.AddTransient<IInventorySearchServiceGateway, InventorySearchServiceGateway>();
            services.AddTransient<IPromotionSearchServiceGateway, PromotionSearchServiceGateway>();
            services.AddTransient<IStoreServiceGateway, StoreServiceGateway>();
            services.AddTransient<IPricingServiceGateway, PricingServiceGateway>();
            services.AddTransient<ITaxProviderSearchServiceGateway, TaxProviderSearchServiceGateway>();
            services.AddTransient<IMarketingPromoServiceGateway, MarketingPromoServiceGateway>();
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


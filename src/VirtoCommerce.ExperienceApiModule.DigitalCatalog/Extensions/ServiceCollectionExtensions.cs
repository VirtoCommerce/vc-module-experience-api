using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using AutoMapper;
using VirtoCommerce.XDigitalCatalog.Services;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCatalog(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSchemaBuilder<DigitalCatalogSchema>();

            graphQlbuilder.AddGraphTypes(typeof(XDigitalCatalogAnchor));
            
            services.AddMediatR(typeof(XDigitalCatalogAnchor));
            //the generic pipeline that is used  for on-the-fly additional data evaluation (prices, inventories, discounts and taxes) for resulting products 
            services.AddPipeline<SearchProductResponse>(builder =>
            {
            //    builder.AddMiddleware(typeof(EvalProductsPricesMiddleware));
            //    builder.AddMiddleware(typeof(EvalProductsDiscountsMiddleware));
            //    builder.AddMiddleware(typeof(EvalProductsTaxMiddleware));
            //    builder.AddMiddleware(typeof(EvalProductsInventoryMiddleware));
            });

            services.AddAutoMapper(typeof(XDigitalCatalogAnchor));

            services.AddServiceGateways<IProductAssociationSearchServiceGateway>(new[]
                {
                    /*typeof(ProductAssociationSearchServiceVirtoCommerce),*/
                    typeof(ProductAssociationSearchServiceCommerceTools)
                });
            services.AddServiceGateways<IInventorySearchServiceGateway>(
                new[]
                {
                    /*typeof(InventorySearchServiceVirtoCommerce), */ typeof(InventorySearchServiceCommerceTools)
                });
            services.AddServiceGateways<IPromotionSearchServiceGateway>(
                new[]
                {
                    /*typeof(PromotionSearchServiceVirtoCommerce), */
                    typeof(PromotionSearchServiceCommerceTools)
                });
            services.AddServiceGateways<IStoreServiceGateway>(
                new[]
                {
                    /*typeof(StoreServiceVirtoCommerce),*/ typeof(StoreServiceCommerceTools)
                });
            services.AddServiceGateways<IPricingServiceGateway>(
                new[]
                {
                    /*typeof(PricingServiceVirtoCommerce),*/ typeof(PricingServiceCommerceTools)
                });

            var gateway = services.BuildServiceProvider().GetService<IPricingServiceGateway>();

            return services;
        }
    }
}

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
                builder.AddMiddleware(typeof(EvalProductsPricesMiddleware));
                builder.AddMiddleware(typeof(EvalProductsDiscountsMiddleware));
                builder.AddMiddleware(typeof(EvalProductsTaxMiddleware));
                builder.AddMiddleware(typeof(EvalProductsInventoryMiddleware));
            });

            services.AddAutoMapper(typeof(XDigitalCatalogAnchor));

            services.AddExpServices(typeof(IExpProductAssociationSearchService)
                , new[] { typeof(ExpProductAssociationSearchService), typeof(ExpProductAssociationSearchServiceNswag) });
            services.AddExpServices(typeof(IExpInventorySearchService), new[] { typeof(ExpInventorySearchService) });
            services.AddExpServices(typeof(IExpPromotionSearchService), new[] { typeof(ExpPromotionSearchService) });
            services.AddExpServices(typeof(IExpStoreService), new[] { typeof(ExpStoreService) });
            services.AddExpServices(typeof(IExpPricingService), new[] { typeof(ExpPricingService) });

            //TODO
            //var serv = services.BuildServiceProvider().GetService<IExpProductAssociationSearchService>();
            //var serv2 = services.BuildServiceProvider().GetService<IExpInventorySearchService>();

            return services;
        }
    }
}

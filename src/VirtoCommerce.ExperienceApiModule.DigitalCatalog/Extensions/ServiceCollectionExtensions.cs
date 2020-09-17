using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using AutoMapper;
using VirtoCommerce.XDigitalCatalog.Mapping;
using AutoMapper.Configuration;

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
                builder.AddMiddleware(typeof(ProductsPricesEvalMiddleware));
                builder.AddMiddleware(typeof(ProductsDiscountsEvalMiddleware));
                builder.AddMiddleware(typeof(ProductsTaxEvalMiddleware));
                builder.AddMiddleware(typeof(ProductsInventoryEvalMiddleware));
            });

            services.AddAutoMapper(typeof(XDigitalCatalogAnchor));


            return services;
        }
    }
}

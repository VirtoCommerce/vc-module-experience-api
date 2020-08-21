using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using AutoMapper;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCatalog(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            //Discover the assembly and  register all mapping profiles through reflection
            //TODO: Seems this is not work need to find out why
            services.AddAutoMapper(typeof(XDigitalCatalogAnchor));

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

            return services;
        }
    }
}

using AutoMapper;
using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCatalog(this IServiceCollection services, IGraphQLBuilder graphQlBuilder)
        {
            var assemblyMarker = typeof(XDigitalCatalogAnchor);
            graphQlBuilder.AddGraphTypes(assemblyMarker);
            services.AddMediatR(assemblyMarker);
            services.AddAutoMapper(assemblyMarker);
            services.AddSchemaBuilders(assemblyMarker);

            // The generic pipeline that is used for on-the-fly additional data evaluation (prices, inventories, discounts and taxes) for resulting products
            services.AddPipeline<SearchProductResponse>(builder =>
            {
                builder.AddMiddleware(typeof(EnsureCatalogProductLoadedMiddleware));
                builder.AddMiddleware(typeof(RemoveNullCatalogProductsMiddleware));
                builder.AddMiddleware(typeof(EvalProductsPricesMiddleware));
                builder.AddMiddleware(typeof(EvalProductsDiscountsMiddleware));
                builder.AddMiddleware(typeof(EvalProductsTaxMiddleware));
                builder.AddMiddleware(typeof(EvalProductsInventoryMiddleware));
                builder.AddMiddleware(typeof(EvalProductsVendorMiddleware));
                builder.AddMiddleware(typeof(EvalProductsWishlistsMiddleware));
            });

            services.AddPipeline<SearchCategoryResponse>(builder =>
            {
                builder.AddMiddleware(typeof(EnsureCategoryLoadedMiddleware));
            });

            return services;
        }
    }
}

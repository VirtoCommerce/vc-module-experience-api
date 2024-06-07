using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.XDigitalCatalog.Core;
using VirtoCommerce.XDigitalCatalog.Core.Models;
using VirtoCommerce.XDigitalCatalog.Data.Index;
using VirtoCommerce.XDigitalCatalog.Data.Middlewares;

namespace VirtoCommerce.XDigitalCatalog.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCatalog(this IServiceCollection services, IGraphQLBuilder graphQlBuilder)
        {
            var assemblyMarkerData = typeof(XDigitalCatalogDataAnchor);
            var assemblyMarkerCore = typeof(XDigitalCatalogCoreAnchor);
            graphQlBuilder.AddGraphTypes(assemblyMarkerCore);
            services.AddMediatR(assemblyMarkerCore, assemblyMarkerData);
            services.AddAutoMapper(assemblyMarkerCore, assemblyMarkerData);
            services.AddSchemaBuilders(assemblyMarkerData);

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
                builder.AddMiddleware(typeof(EnsurePropertyMetadataLoadedMiddleware));
                ////builder.AddMiddleware(typeof(EvalProductsWishlistsMiddleware));
            });

            services.AddPipeline<SearchCategoryResponse>(builder =>
            {
                builder.AddMiddleware(typeof(EnsureCategoryLoadedMiddleware));
            });

            services.AddPipeline<IndexSearchRequestBuilder>(builder =>
            {
                builder.AddMiddleware(typeof(EvalSearchRequestUserGroupsMiddleware));
            });

            services.AddPipeline<InventorySearchCriteria>();

            return services;
        }
    }
}

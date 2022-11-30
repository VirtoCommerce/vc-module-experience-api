using AutoMapper;
using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XDigitalCatalog.Schemas.Inventory;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCatalog(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSchemaBuilder<DigitalCatalogSchema>();
            services.AddSchemaBuilder<InventorySchema>();

            graphQlbuilder.AddGraphTypes(typeof(XDigitalCatalogAnchor));

            services.AddMediatR(typeof(XDigitalCatalogAnchor));
            //the generic pipeline that is used  for on-the-fly additional data evaluation (prices, inventories, discounts and taxes) for resulting products
            services.AddPipeline<SearchProductResponse>(builder =>
            {
                builder.AddMiddleware(typeof(EnsureCatalogProductLoadedMiddleware));
                builder.AddMiddleware(typeof(RemoveNullCatalogProductsMiddleware));
                builder.AddMiddleware(typeof(EvalProductsPricesMiddleware));
                builder.AddMiddleware(typeof(EvalProductsDiscountsMiddleware));
                builder.AddMiddleware(typeof(EvalProductsTaxMiddleware));
                builder.AddMiddleware(typeof(EvalProductsInventoryMiddleware));
                builder.AddMiddleware(typeof(EvalProductsVendorMiddleware));
            });

            services.AddPipeline<SearchCategoryResponse>(builder =>
            {
                builder.AddMiddleware(typeof(EnsureCategoryLoadedMiddleware));
            });

            services.AddAutoMapper(typeof(XDigitalCatalogAnchor));

            return services;
        }
    }
}

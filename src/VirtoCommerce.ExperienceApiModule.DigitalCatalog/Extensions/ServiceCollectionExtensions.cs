using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCatalog(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSchemaBuilder<DigitalCatalogSchema>();

            // TODO: check if anchor loading is working, remove direct schema registration
            services.AddSchemaType<CategoryType>();

            // TODO: check if this work remove upper code
            graphQlbuilder.AddGraphTypes(typeof(XDigitalCatalogAnchor));

            services.AddMediatR(typeof(XDigitalCatalogAnchor));

            return services;
        }
    }
}

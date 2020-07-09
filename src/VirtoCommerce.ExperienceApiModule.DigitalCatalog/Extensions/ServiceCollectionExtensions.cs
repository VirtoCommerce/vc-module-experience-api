using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCatalog(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSchemaBuilder<DigitalCatalogSchema>();

            graphQlbuilder.AddGraphTypes(typeof(XDigitalCatalogAnchor));

            services.AddMediatR(typeof(XDigitalCatalogAnchor));

            return services;
        }
    }
}

using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.XCMS.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCMS(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            graphQlbuilder.AddGraphTypes(typeof(XCMSAnchor));

            services.AddSchemaBuilder<ContentSchema>();

            services.AddMediatR(typeof(ContentSchema));

            return services;
        }
    }
}

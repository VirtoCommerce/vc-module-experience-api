using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XProfile.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXProfile(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSchemaBuilder<ProfileSchema>();


            graphQlbuilder.AddGraphTypes(typeof(XProfileAnchor));


            services.AddMediatR(typeof(XProfileAnchor));

            services.AddTransient<IOrganizationAggregateRepository, OrganizationAggregateRepository>();
            services.AddTransient<IContactAggregateRepository, ContactAggregateRepository>();

            return services;
        }
    }
}

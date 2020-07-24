using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XOrder.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXOrder(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSchemaBuilder<OrderSchema>();

            graphQlbuilder.AddGraphTypes(typeof(XOrderAnchor));

            services.AddMediatR(typeof(XOrderAnchor));

            services.AddTransient<ICustomerOrderAggregateRepository, CustomerOrderAggregateRepository>();


            return services;
        }
    }
}

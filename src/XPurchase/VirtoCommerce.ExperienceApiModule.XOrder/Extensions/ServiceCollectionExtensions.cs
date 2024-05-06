using System;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.XOrder.Authorization;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXOrder(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            var assemblyMarker = typeof(XOrderAnchor);
            graphQlbuilder.AddGraphTypes(assemblyMarker);
            services.AddMediatR(assemblyMarker);
            services.AddAutoMapper(assemblyMarker);
            services.AddSchemaBuilders(assemblyMarker);

            services.AddTransient<ICustomerOrderAggregateRepository, CustomerOrderAggregateRepository>();
            services.AddSingleton<IAuthorizationHandler, CanAccessOrderAuthorizationHandler>();

            services.AddTransient<CustomerOrderAggregate>();
            services.AddTransient<Func<CustomerOrderAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<CustomerOrderAggregate>());

            return services;
        }
    }
}

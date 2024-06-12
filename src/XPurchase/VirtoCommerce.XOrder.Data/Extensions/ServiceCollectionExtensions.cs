using System;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.XOrder.Core;
using VirtoCommerce.XOrder.Core.Authorization;
using VirtoCommerce.XOrder.Core.Services;
using VirtoCommerce.XOrder.Data.Services;

namespace VirtoCommerce.XOrder.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXOrder(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            var assemblyMarkerCore = typeof(XOrderCoreAnchor);
            var assemblyMarkerData = typeof(XOrderDataAnchor);
            graphQlbuilder.AddGraphTypes(assemblyMarkerCore);
            services.AddMediatR(assemblyMarkerCore, assemblyMarkerData);
            services.AddAutoMapper(assemblyMarkerCore, assemblyMarkerData);
            services.AddSchemaBuilders(assemblyMarkerData);

            services.AddTransient<ICustomerOrderAggregateRepository, CustomerOrderAggregateRepository>();
            services.AddSingleton<IAuthorizationHandler, CanAccessOrderAuthorizationHandler>();

            services.AddTransient<CustomerOrderAggregate>();
            services.AddTransient<Func<CustomerOrderAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<CustomerOrderAggregate>());

            return services;
        }
    }
}

using System;
using GraphQL.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.XOrder.Core;
using VirtoCommerce.XOrder.Core.Authorization;
using VirtoCommerce.XOrder.Core.Services;
using VirtoCommerce.XOrder.Data.Middlewares;
using VirtoCommerce.XOrder.Data.Services;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XOrder.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXOrder(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            graphQlbuilder.AddSchema(typeof(XOrderCoreAssemblyMarker), typeof(XOrderDataAssemblyMarker));

            services.AddTransient<ICustomerOrderAggregateRepository, CustomerOrderAggregateRepository>();
            services.AddSingleton<IAuthorizationHandler, CanAccessOrderAuthorizationHandler>();

            services.AddTransient<CustomerOrderAggregate>();
            services.AddTransient<Func<CustomerOrderAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<CustomerOrderAggregate>());

            services.AddPipeline<PromotionEvaluationContextCartMap>(builder =>
            {
                builder.AddMiddleware(typeof(EvalPromoContextOrderMiddleware));
            });

            return services;
        }
    }
}

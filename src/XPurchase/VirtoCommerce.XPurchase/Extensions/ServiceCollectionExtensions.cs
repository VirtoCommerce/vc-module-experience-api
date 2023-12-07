using System;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.XPurchase.Authorization;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXPurchase(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            var assemblyMarker = typeof(XPurchaseAnchor);
            graphQlbuilder.AddGraphTypes(assemblyMarker);
            services.AddMediatR(assemblyMarker);
            services.AddAutoMapper(assemblyMarker);
            services.AddSchemaBuilders(assemblyMarker);

            services.AddSingleton<IAuthorizationHandler, CanAccessCartAuthorizationHandler>();
            services.AddTransient<ICartAggregateRepository, CartAggregateRepository>();
            services.AddTransient<ICartValidationContextFactory, CartValidationContextFactory>();
            services.AddTransient<ICartAvailMethodsService, CartAvailMethodsService>();
            services.AddTransient<ICartProductService, CartProductService>();
            services.AddTransient<CartAggregate>();
            services.AddTransient<Func<CartAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<CartAggregate>());
            services.AddSingleton<ICartResponseGroupParser, CartResponseGroupParser>();

            return services;
        }
    }
}

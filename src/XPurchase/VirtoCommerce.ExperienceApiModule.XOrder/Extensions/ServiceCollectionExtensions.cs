using System;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XOrder.Authorization;
using VirtoCommerce.ExperienceApiModule.XOrder.Schemas;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXOrder(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSingleton<Func<SignInManager<ApplicationUser>>>(provider => () => provider.GetService<SignInManager<ApplicationUser>>());
            services.AddSingleton<ISchemaBuilder, OrderSchema>();

            graphQlbuilder.AddGraphTypes(typeof(XOrderAnchor));

            services.AddMediatR(typeof(XOrderAnchor));

            services.AddTransient<ICustomerOrderAggregateRepository, CustomerOrderAggregateRepository>();
            services.AddSingleton<IAuthorizationHandler, CanAccessOrderAuthorizationHandler>();

            return services;
        }
    }
}

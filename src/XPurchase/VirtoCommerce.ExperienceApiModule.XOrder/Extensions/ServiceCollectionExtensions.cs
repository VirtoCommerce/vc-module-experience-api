using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<ISchemaBuilder>(provider => {
                return new OrderSchema(
                    provider.GetService<IMediator>(),
                    provider.GetService<IAuthorizationService>(),
                    () => provider.CreateScope().ServiceProvider.GetService<SignInManager<ApplicationUser>>()
                    );
            });

            graphQlbuilder.AddGraphTypes(typeof(XOrderAnchor));

            services.AddMediatR(typeof(XOrderAnchor));

            services.AddTransient<ICustomerOrderAggregateRepository, CustomerOrderAggregateRepository>();
            services.AddSingleton<IAuthorizationHandler, CanAccessOrderAuthorizationHandler>();

            return services;
        }
    }
}

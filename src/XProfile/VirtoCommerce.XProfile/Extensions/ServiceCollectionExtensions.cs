using AutoMapper;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.ExperienceApiModule.XProfile.Authorization;
using VirtoCommerce.ExperienceApiModule.XProfile.Schemas;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXProfile(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            services.AddSchemaBuilder<ProfileSchema>();

            graphQlbuilder.AddGraphTypes(typeof(XProfileAnchor));

            services.AddMediatR(typeof(XProfileAnchor));
            services.AddSingleton<IMemberAggregateFactory, MemberAggregateFactory>();
            services.AddTransient<NewContactValidator>();
            services.AddTransient<IMemberAggregateRootRepository, MemberAggregateRootRepository>();
            services.AddTransient<IOrganizationAggregateRepository, OrganizationAggregateRepository>();
            services.AddTransient<IContactAggregateRepository, ContactAggregateRepository>();
            services.AddSingleton<IAuthorizationHandler, ProfileAuthorizationHandler>();

            services.AddAutoMapper(typeof(XProfileAnchor));

            return services;
        }
    }
}

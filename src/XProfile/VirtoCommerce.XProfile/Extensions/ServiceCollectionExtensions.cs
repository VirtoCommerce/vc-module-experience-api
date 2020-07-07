using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XProfile.Schemas;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXProfile(this IServiceCollection services)
        {
            services.AddSchemaBuilder<ProfileSchema>();

            services.AddMediatR(typeof(XProfileAnchor));

            services.AddTransient<IMemberServiceX, MemberServiceX>();

            services.AddTransient<IOrganizationAggregateRepository, OrganizationAggregateRepository>();
            services.AddTransient<IContactAggregateRepository, ContactAggregateRepository>();

            return services;
        }
    }
}

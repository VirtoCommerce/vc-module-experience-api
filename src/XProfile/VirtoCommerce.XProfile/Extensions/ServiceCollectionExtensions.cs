using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XProfile.Schemas;
using MediatR;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;
using System;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXProfile(this IServiceCollection services)
        {
            //services.AddSchemaType<UserUpdateInfoInputType>();
            services.AddSchemaType<OrganizationType>();
            services.AddSchemaType<ContactType>();
            services.AddSchemaType<InputCreateOrganizationType>();
            services.AddSchemaType<InputUpdateOrganizationType>();

            services.AddSchemaBuilder<ProfileSchema>();

            services.AddMediatR(typeof(XProfileAnchor));

            services.AddTransient<IMemberServiceX, MemberServiceX>();

            services.AddTransient<IOrganizationAggregateRepository, OrganizationAggregateRepository>();
            services.AddTransient<OrganizationAggregate>();
            services.AddTransient<Func<OrganizationAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<OrganizationAggregate>());

            services.AddTransient<IContactAggregateRepository, ContactAggregateRepository>();
            services.AddTransient<ContactAggregate>();
            services.AddTransient<Func<ContactAggregate>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ContactAggregate>());

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XProfile.Schemas;
using MediatR;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

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
            services.AddSchemaType<InputCreateContactType>();
            services.AddSchemaType<InputUpdateContactType>();
            services.AddSchemaType<InputUpdateContactAddressType>();
            services.AddSchemaType<InputDeleteContactType>();

            services.AddSchemaBuilder<ProfileSchema>();

            services.AddMediatR(typeof(XProfileAnchor));

            services.AddTransient<IMemberServiceX, MemberServiceX>();

            services.AddTransient<IOrganizationAggregateRepository, OrganizationAggregateRepository>();
            services.AddTransient<IContactAggregateRepository, ContactAggregateRepository>();

            return services;
        }
    }
}

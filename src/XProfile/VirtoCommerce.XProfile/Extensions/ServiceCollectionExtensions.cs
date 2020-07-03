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

            services.AddSchemaBuilder<ProfileSchema>();

            services.AddMediatR(typeof(XProfileAnchor));

            services.AddTransient<IMemberServiceX, MemberServiceX>();

            return services;
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApi.ProfileModule.Core.Models;
using VirtoCommerce.ExperienceApi.ProfileModule.Data.Handlers;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ExperienceApi.ProfileModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(typeof(LoadProfileRequestHandler));
            //serviceCollection.AddAutoMapper(typeof(LoadProfileRequestHandler));
            serviceCollection.AddSchemaBuilder<ProfileSchema>();
            serviceCollection.AddSchemaType<ProfileType>();
            serviceCollection.AddSchemaType<ContactType>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            // Method intentionally left empty.
        }

        public void Uninstall()
        {
            // do nothing in here
        }

    }

}

using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ExperienceApiModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ProductAssociationType>();
            serviceCollection.AddSingleton<PropertyType>();
            serviceCollection.AddSingleton<ProductType>();
            serviceCollection.AddSingleton<PropertyTypeEnum>();
            serviceCollection.AddSingleton<RootQuery>();
            serviceCollection.AddSingleton<ISchema, RootSchema>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
        }

        public void Uninstall()
        {
        }
               
    }
}


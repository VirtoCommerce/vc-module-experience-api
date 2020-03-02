using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.CoreModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<PropertyType>();
            serviceCollection.AddSingleton<ProductType>();
            serviceCollection.AddSingleton<ProductsQuery>();
            serviceCollection.AddSingleton<ISchema, ProductsSchema>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
        }

        public void Uninstall()
        {
        }
               
    }
}


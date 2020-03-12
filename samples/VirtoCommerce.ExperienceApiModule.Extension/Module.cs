using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ExperienceApiModule.Extension
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            //// Replace
            //// IFoo -> FooB
            //var descriptor = new ServiceDescriptor(typeof(ProductType), typeof(ProductType2), ServiceLifetime.Singleton);
            //serviceCollection.Replace(descriptor);

            //serviceCollection.AddSingleton<ProductType, ProductType2>();
            //serviceCollection.AddSingleton<ISchema, RootSchema2>();

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            //var schema = appBuilder.ApplicationServices.GetService<ISchema>();
            //schema.Initialize();
        }

        public void Uninstall()
        {
        }
               
    }
}


using System;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PetsStoreClient;
using PetsStoreClient.Nswag;
using VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection services)
        {


            #region Extension scenarios

            #region Type override: add a new properties
            services.AddSchemaType<CartType2>().OverrideType<CartType, CartType2>();

            //Register GraphQL ProductType2 type and override exists ProductType
            services.AddSchemaType<ProductType2>().OverrideType<ProductType, ProductType2>();
            //Override domain type ExpProduct -> ExpProduct2
            AbstractTypeFactory<ExpProduct>.OverrideType<ExpProduct, ExpProduct2>();
            services.AddSchemaType<InventoryType>();
            services.AddSchemaType<InputUpdateInventoryType>();
            //Register custom schema
            services.AddSchemaBuilder<CustomSchema>();
            #endregion

            #region UseCase OnTheFlyEvaluation: evaluate product inventories on the fly 
            //services.AddSingleton(typeof(IRequestPostProcessor<,>), typeof(EvalInventoriesForProductsPipelineBehaviour<,>));
            #endregion

            #region  UseCase CombinedDataSource: paginating data from multiple sources (VC catalog and Pets store)
            //services.AddTransient<IPetsSearchService, PetsSearchService>();
            //services.AddHttpClient<PetstoreClient>(c => c.BaseAddress = new Uri("http://petstore.swagger.io/v2/"));
            //services.AddSingleton(typeof(IRequestPostProcessor<,>), typeof(VcAndPetsSearchPipelineBehaviour<,>));
            #endregion

            #region UseCase DataSourceSubstitution: replace data source to another
            //serviceCollection.AddSingleton<IProductSearchService, PetsProductSearchService>();
            #endregion

            #endregion

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {

       
        }

        public void Uninstall()
        {
        }
               
    }
}


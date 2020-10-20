using System;
using System.Linq;
using AutoMapper;
using GraphQL.Server;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PetsStoreClient;
using PetsStoreClient.Nswag;
using VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension;
using VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Queries;
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
            //use such lines to override exists query or command handler
            services.AddTransient<IRequestHandler<GetCartQuery, CartAggregate>, CustomGetCartQueryHandler>();

            services.AddGraphQL().AddGraphTypes(typeof(XExtensionAnchor));
            //Register custom schema
            services.AddSchemaBuilder<CustomSchema>();

            //GraphQL schema overrides 
            services.AddSchemaType<CartType2>().OverrideType<CartType, CartType2>();
            services.AddSchemaType<ProductType2>().OverrideType<ProductType, ProductType2>();
            //Domain types overrides
            AbstractTypeFactory<ExpProduct>.OverrideType<ExpProduct, ExpProduct2>();

            services.AddAutoMapper(typeof(XExtensionAnchor));
            services.AddMediatR(typeof(XExtensionAnchor));
            #endregion

            #region UseCase OnTheFlyEvaluation: load product inventories from the index instead of DB
            services.AddPipeline<SearchProductResponse>(builder =>
            {
                builder.ReplaceMiddleware(typeof(EvalProductsInventoryMiddleware), typeof(LoadProductsInventoriesFromSomewhereMiddleware));
            });
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


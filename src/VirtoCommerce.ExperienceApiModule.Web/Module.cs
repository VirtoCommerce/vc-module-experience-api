using System;
using AutoMapper;
using GraphQL.Types;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PetsStoreClient;
using PetsStoreClient.Nswag;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;
using VirtoCommerce.ExperienceApiModule.Data.Handlers;
using VirtoCommerce.ExperienceApiModule.Data.Pipeline;
using VirtoCommerce.ExperienceApiModule.Data.UseCases.OnTheFly;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using PropertyType = VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas.PropertyType;

namespace VirtoCommerce.ExperienceApiModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {

            AbstractTypeFactory<CatalogProduct>.OverrideType<CatalogProduct, CatalogProduct2>();
            serviceCollection.AddMediatR(typeof(HandlersAnchor));

            serviceCollection.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LogPipelineBehaviour<,>));
            serviceCollection.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
            //serviceCollection.AddSingleton(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>));

            #region on the fly  price evaluation  

            serviceCollection.AddSingleton(typeof(IRequestPostProcessor<,>), typeof(EvalPriceForProductsPipelineBehaviour<,>));

            #endregion

            #region  paginating data from multiple sources (VC catalog and Pets store)

            serviceCollection.AddSingleton(typeof(IRequestPostProcessor<,>), typeof(VcAndPetsSearchPipelineBehaviour<,>));

            #endregion


            #region  replace data source
            //serviceCollection.AddSingleton<IProductSearchService, PetsProductSearchService>();
            #endregion

            serviceCollection.AddSingleton<ProductAssociationType>();
            serviceCollection.AddSingleton<PropertyType>();
            serviceCollection.AddSingleton<ProductType>();
            serviceCollection.AddSingleton<PropertyTypeEnum>();
            serviceCollection.AddSingleton<RootQuery>();
            serviceCollection.AddSingleton<ISchema, RootSchema>();

            serviceCollection.AddTransient<IPetsSearchService, PetsSearchService>();
            serviceCollection.AddHttpClient<PetstoreClient>(c => c.BaseAddress = new Uri("http://petstore.swagger.io/v2/"));

            //Discover the assembly and  register all mapping profiles through reflection
            serviceCollection.AddAutoMapper(typeof(AnchorType));
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
        }

        public void Uninstall()
        {
        }
               
    }
}


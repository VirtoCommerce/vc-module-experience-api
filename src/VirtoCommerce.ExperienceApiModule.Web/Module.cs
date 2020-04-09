using System;
using AutoMapper;
using GraphQL.Server;
using GraphQL.Types;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PetsStoreClient;
using PetsStoreClient.Nswag;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL;
using VirtoCommerce.ExperienceApiModule.Data.Handlers;
using VirtoCommerce.ExperienceApiModule.Data.Pipeline;
using VirtoCommerce.ExperienceApiModule.Extension;
using VirtoCommerce.ExperienceApiModule.Extension.GraphQL.Schemas;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.OnTheFly;
using VirtoCommerce.ExperienceApiModule.GraphQLEx;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using schema = VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;
namespace VirtoCommerce.ExperienceApiModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection services)
        {

            services.AddMediatR(typeof(HandlersAnchor));

            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LogPipelineBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
            //serviceCollection.AddSingleton(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>));
                      
            services.AddTransient<IPetsSearchService, PetsSearchService>();
            services.AddHttpClient<PetstoreClient>(c => c.BaseAddress = new Uri("http://petstore.swagger.io/v2/"));

            //Discover the assembly and  register all mapping profiles through reflection
            services.AddAutoMapper(typeof(AnchorType));


            //Register .NET GraphQL server
            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
                _.ExposeExceptions = true;
            }).AddNewtonsoftJson(deserializerSettings => { }, serializerSettings => { })
            .AddUserContextBuilder(context => new GraphQLUserContext { User = context.User })
            .AddRelayGraphTypes()
            .AddGraphTypes(typeof(SchemaAnchor))
            .AddDataLoader()
            .AddPermissionAuthorization()
            .AddGraphShemaBuilders(typeof(SchemaAnchor));


            #region Extension scenarios

            #region Type override: add a new properties
            //Override domain type CatalogProduct -> CatalogProduct2
            AbstractTypeFactory<CatalogProduct>.OverrideType<CatalogProduct, CatalogProduct2>();

            //Override GraphType  ProductType -> ProductType2
            services.AddSingleton<ProductType2>();
            GraphTypeExtenstionHelper.OverrideGraphType<schema.ProductType, ProductType2>();
            #endregion

            #region UseCase OnTheFlyEvaluation: evaluate product prices on the fly 
            services.AddSingleton(typeof(IRequestPostProcessor<,>), typeof(EvalPriceForProductsPipelineBehaviour<,>));
            #endregion

            #region  UseCase CombinedDataSource: paginating data from multiple sources (VC catalog and Pets store)
            services.AddSingleton(typeof(IRequestPostProcessor<,>), typeof(VcAndPetsSearchPipelineBehaviour<,>));
            #endregion

            #region UseCase DataSourceSubstitution: replace data source to another
            //serviceCollection.AddSingleton<IProductSearchService, PetsProductSearchService>();
            #endregion
            #endregion



        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {

            // add http for Schema at default url /graphql
            appBuilder.UseGraphQL<ISchema>();

            // use graphql-playground at default url /ui/playground
            appBuilder.UseGraphQLPlayground();

        }

        public void Uninstall()
        {
        }
               
    }
}


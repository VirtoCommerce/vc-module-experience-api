using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GraphQL.Server;
using GraphQL.Types;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetsStoreClient;
using PetsStoreClient.Nswag;
using Polly;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL;
using VirtoCommerce.ExperienceApiModule.Data.Handlers;
using VirtoCommerce.ExperienceApiModule.Data.Index;
using VirtoCommerce.ExperienceApiModule.Data.Pipeline;
using VirtoCommerce.ExperienceApiModule.Extension;
using VirtoCommerce.ExperienceApiModule.Extension.GraphQL.Schemas;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.OnTheFly;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Configuration;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Handlers;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Requests;
using VirtoCommerce.ExperienceApiModule.GraphQLEx;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.SearchModule.Core.Model;
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
            services.AddSingleton<PriceType>();
            GraphTypeExtenstionHelper.OverrideGraphType<schema.ProductType, ProductType2>();
            #endregion

            #region UseCase OnTheFlyEvaluation: evaluate product prices on the fly 
            //services.AddSingleton(typeof(IRequestPostProcessor<,>), typeof(EvalPriceForProductsPipelineBehaviour<,>));
            #endregion

            #region  UseCase CombinedDataSource: paginating data from multiple sources (VC catalog and Pets store)
            services.AddTransient<IPetsSearchService, PetsSearchService>();
            services.AddHttpClient<PetstoreClient>(c => c.BaseAddress = new Uri("http://petstore.swagger.io/v2/"));
            //services.AddSingleton(typeof(IRequestPostProcessor<,>), typeof(VcAndPetsSearchPipelineBehaviour<,>));
            #endregion

            #region UseCase DataSourceSubstitution: replace data source to another
            //serviceCollection.AddSingleton<IProductSearchService, PetsProductSearchService>();
            #endregion

            #region UseCase ProductRecommendations:  aggregation some responses received from downstreams and contains only product  identifiers with actual product data
            services.AddMediatR(typeof(GetRecommendationsRequestHandler));
            AbstractTypeFactory<DownstreamResponse>.RegisterType<GetRecommendationsResponse>();
            services.AddSingleton<ISchemaBuilder, ProductRecommendationQuery>();
            services.AddSingleton<ProductRecommendationType>();
            services.AddSingleton<IContentRenderer, LiquidContentRenderer>();

            // Build an intermediate service provider
            var sp = services.BuildServiceProvider();

            // Resolve the services from the service provider
            var configuration = sp.GetService<IConfiguration>();

            services.AddOptions<RecommendationOptions>().Bind(configuration.GetSection("Recommendations"))
                .ValidateDataAnnotations().PostConfigure(opts =>
                                                    {
                                                        //match connection for scenario
                                                        foreach(var scenario in opts.Scenarios)
                                                        {
                                                            scenario.Connection = opts.Connections.FirstOrDefault(x => x.Name.EqualsInvariant(scenario.ConnectionName));
                                                        }
                                                    });
            services.AddHttpClient<IDownstreamRequestSender, DownstreamRequestSender>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            #endregion
            #endregion



            services.AddTransient<ProductIndexBuilder>();

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {

            // add http for Schema at default url /graphql
            appBuilder.UseGraphQL<ISchema>();

            // use graphql-playground at default url /ui/playground
            appBuilder.UseGraphQLPlayground();

            //#region Search

            //var productIndexingConfigurations = appBuilder.ApplicationServices.GetServices<IndexDocumentConfiguration>();
            //if (productIndexingConfigurations != null)
            //{
            //    var nestedProductDocSource = new IndexDocumentSource
            //    {
            //        DocumentBuilder = appBuilder.ApplicationServices.GetService<ProductIndexBuilder>(),
            //        ChangesProvider = new DocumentChangesProviderStub()
            //    };

            //    foreach (var configuration in productIndexingConfigurations.Where(c => c.DocumentType == KnownDocumentTypes.Product))
            //    {
            //        if (configuration.RelatedSources == null)
            //        {
            //            configuration.RelatedSources = new List<IndexDocumentSource>();
            //        }
            //        configuration.RelatedSources.Add(nestedProductDocSource);                   
            //    }
            //}

            //#endregion

        }

        public void Uninstall()
        {
        }
               
    }
}


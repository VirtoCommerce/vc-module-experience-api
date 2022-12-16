using AutoMapper;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Exp.ExtensionSamples.Commands;
using VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries;
using VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Schemas;
using VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Validators;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.ExperienceApiModule.XOrder.Schemas;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Queries;
using VirtoCommerce.XPurchase.Schemas;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection services)
        {
            #region Extension scenarios

            #region Type override: add a new properties
            //use such lines to override exists query and command handler
            services.OverrideQueryType<GetCartQuery, GetCartQueryExtended>().WithQueryHandler<CustomGetCartQueryHandler>();
            services.OverrideQueryType<SearchOrderQuery, ExtendedSearchOrderQuery>().WithQueryHandler<ExtendedSearchOrderQueryHandler>();
            services.OverrideArgumentType<OrderQueryConnectionArguments, ExtendedOrderQueryConnectionArguments>();
            services.AddGraphQL(_ =>
            {
                //It is important to pass the GraphQLOptions configure action, because the default parameters used in xAPI module won't be used after this call
                _.EnableMetrics = false;

            })
                .AddGraphTypes(typeof(XExtensionAnchor))
                .AddErrorInfoProvider(options =>
                {
                    options.ExposeExtensions = true;
                    options.ExposeExceptionStackTrace = true;
                });
            //Register custom schema
            services.AddSchemaBuilder<CustomSchema>();

            //GraphQL schema overrides
            services.AddSchemaType<CartType2>().OverrideType<CartType, CartType2>();
            services.AddSchemaType<ProductType2>().OverrideType<ProductType, ProductType2>();
            services.AddSchemaType<InputRemoveCartType2>().OverrideType<InputRemoveCartType, InputRemoveCartType2>();
            services.OverrideCommandType<RemoveCartCommand, RemoveCartCommandExtended>().WithCommandHandler<RemoveCartCommandHandlerExtended>();

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
            #region Extension scenarios
            #region UseCase Validators: Extend validation logic / replace validators by custom ones
            // Example: replace cart validator
            AbstractTypeFactory<CartValidator>.OverrideType<CartValidator, CartValidator2>(); 
            #endregion
            #endregion
        }

        public void Uninstall()
        {
        }
    }
}


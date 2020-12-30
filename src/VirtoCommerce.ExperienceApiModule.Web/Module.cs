using System;
using AutoMapper;
using GraphQL.Execution;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XProfile.Middlewares;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Middlewares;

namespace VirtoCommerce.ExperienceApiModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection services)
        {
            //Register .NET GraphQL server
            var graphQlBuilder = services.AddGraphQL(_ =>
            {
                _.EnableMetrics = false;
            }).AddNewtonsoftJson(deserializerSettings => { }, serializerSettings => { })
            .AddErrorInfoProvider(options =>
            {
                options.ExposeExtensions = true;
                options.ExposeExceptionStackTrace = true;
            })
            .AddUserContextBuilder(context => new GraphQLUserContext { User = context.User })
            .AddRelayGraphTypes()
            .AddDataLoader()
            .AddGraphTypes(typeof(XCoreAnchor));

            //Register custom GraphQL dependencies
            services.AddPermissionAuthorization();

            services.AddSingleton<ISchema, SchemaFactory>();

            //Register all xApi boundaries
            services.AddXCatalog(graphQlBuilder);
            services.AddXProfile(graphQlBuilder);
            services.AddXPurchase(graphQlBuilder);
            services.AddXOrder(graphQlBuilder);

            //TODO: Remove after update GraphQL.net to 3.2.0 version.
            //VP-6356 DateTime field types for GraphQL schema do not return time in result
            GraphTypeTypeRegistry.Register<DateTime, DateTimeGraphType>();


            services.AddSingleton<IStoreCurrencyResolver, StoreCurrencyResolver>();

            services.AddAutoMapper(ModuleInfo.Assembly);

            #region Pipelines
            services.AddPipeline<PromotionEvaluationContext>(builder =>
               {
                   builder.AddMiddleware(typeof(LoadUserToEvalContextMiddleware));
                   builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
               });
            services.AddPipeline<TaxEvaluationContext>(builder =>
            {
                builder.AddMiddleware(typeof(LoadUserToEvalContextMiddleware));
                builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
            });
            services.AddPipeline<PriceEvaluationContext>(builder =>
            {
                builder.AddMiddleware(typeof(LoadUserToEvalContextMiddleware));
                builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
            }); 
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
            // Method intentionally left empty.
        }
    }   
}

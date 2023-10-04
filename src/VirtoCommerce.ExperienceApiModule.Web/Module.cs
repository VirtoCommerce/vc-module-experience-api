using GraphQL.Server;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Web.Extensions;
using VirtoCommerce.ExperienceApiModule.XCMS.Extensions;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Middlewares;

namespace VirtoCommerce.ExperienceApiModule.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetryProcessor<IgnorePlainGraphQLTelemetryProcessor>();
            // register custom executor with app insight wrapper
            services.AddTransient(typeof(IGraphQLExecuter<>), typeof(CustomGraphQLExecuter<>));

            //Register .NET GraphQL server
            var graphQlBuilder = services.AddGraphQL(options =>
            {
                options.EnableMetrics = false;
            })
            .AddNewtonsoftJson(deserializerSettings => { }, serializerSettings => { })
            .AddErrorInfoProvider(options =>
            {
                options.ExposeExtensions = true;
                options.ExposeExceptionStackTrace = true;
            })
            .AddUserContextBuilder(context => context.BuildGraphQLUserContext())
            .AddRelayGraphTypes()
            .AddDataLoader();

            //Register custom GraphQL dependencies
            services.AddPermissionAuthorization();

            services.AddSingleton<ISchema, SchemaFactory>();

            //Register all xApi boundaries
            services.AddXCatalog(graphQlBuilder);
            services.AddXCore(graphQlBuilder);
            services.AddXPurchase(graphQlBuilder);
            services.AddXOrder(graphQlBuilder);
            services.AddXCMS(graphQlBuilder);

            services.AddAutoMapper(ModuleInfo.Assembly);

            services.AddTransient<LoadUserToEvalContextService>();

            services.AddDistributedLockService(Configuration);

            services.AddPipeline<PromotionEvaluationContext>(builder =>
            {
                builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
            });
            services.AddPipeline<TaxEvaluationContext>(builder =>
            {
                builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
            });
            services.AddPipeline<PriceEvaluationContext>(builder =>
            {
                builder.AddMiddleware(typeof(LoadCartToEvalContextMiddleware));
            });

            services.AddOptions<GraphQLPlaygroundOptions>().Bind(Configuration.GetSection("VirtoCommerce:GraphQLPlayground")).ValidateDataAnnotations();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            // add http for Schema at default url /graphql
            appBuilder.UseGraphQL<ISchema>();

            var playgroundOptions = appBuilder.ApplicationServices.GetRequiredService<IOptions<GraphQLPlaygroundOptions>>().Value;
            if (playgroundOptions.Enable)
            {
                // Use GraphQL Playground at default URL /ui/playground
                appBuilder.UseGraphQLPlayground();
            }
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }
    }
}

using GraphQL;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Validation.Rules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Validation;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions;
using VirtoCommerce.ExperienceApiModule.Data.Extensions;
using VirtoCommerce.ExperienceApiModule.Web.Extensions;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Data.Extensions;
using VirtoCommerce.XOrder.Data.Extensions;
using VirtoCommerce.XPurchase.Data.Extensions;

namespace VirtoCommerce.ExperienceApiModule.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        private const string GraphQLPlaygroundConfigKey = "VirtoCommerce:GraphQLPlayground";
        private const string GraphQLWebSocketConfigKey = "VirtoCommerce:GraphQLWebSocket";
        private const string StoresConfigKey = "VirtoCommerce:Stores";

        private bool IsSchemaIntrospectionEnabled
        {
            get
            {
                return Configuration.GetValue<bool>($"{GraphQLPlaygroundConfigKey}:{nameof(GraphQLPlaygroundOptions.Enable)}");
            }
        }

        public void Initialize(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetryProcessor<IgnorePlainGraphQLTelemetryProcessor>();
            // register custom executor with app insight wrapper
            services.AddTransient(typeof(IGraphQLExecuter<>), typeof(CustomGraphQLExecuter<>));
            services.AddSingleton<IDocumentExecuter, SubscriptionDocumentExecuter>();

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
            .AddCustomWebSockets()
            .AddDataLoader()
            .AddCustomValidationRule<ContentTypeValidationRule>();

            if (!IsSchemaIntrospectionEnabled)
            {
                graphQlBuilder.ReplaceValidationRule<KnownTypeNames, CustomKnownTypeNames>();
                graphQlBuilder.ReplaceValidationRule<FieldsOnCorrectType, CustomFieldsOnCorrectType>();
                graphQlBuilder.ReplaceValidationRule<KnownArgumentNames, CustomKnownArgumentNames>();
            }

            //Register all xApi boundaries
            services.AddXCore(graphQlBuilder, Configuration);
            services.AddXCatalog(graphQlBuilder);
            services.AddXPurchase(graphQlBuilder);

            services.AddXOrder(graphQlBuilder);

            //services.AddXCMS(graphQlBuilder);

            services.AddAutoMapper(ModuleInfo.Assembly);

            services.Configure<GraphQLPlaygroundOptions>(Configuration.GetSection(GraphQLPlaygroundConfigKey));
            services.Configure<GraphQLWebSocketOptions>(Configuration.GetSection(GraphQLWebSocketConfigKey));
            services.Configure<StoresOptions>(Configuration.GetSection(StoresConfigKey));
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var serviceProvider = appBuilder.ApplicationServices;

            // this is required for websockets support
            appBuilder.UseWebSockets();

            // use websocket middleware for ISchema at default path /graphql
            appBuilder.UseGraphQLWebSockets<ISchema>();

            // add http for Schema at default url /graphql
            appBuilder.UseGraphQL<ISchema>();

            if (IsSchemaIntrospectionEnabled)
            {
                // Use GraphQL Playground at default URL /ui/playground
                appBuilder.UseGraphQLPlayground();
            }

            // settings
            var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);
            settingsRegistrar.RegisterSettingsForType(ModuleConstants.Settings.StoreLevelSettings, nameof(Store));
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }
    }
}

using System;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RecommendationsGatewayModule.Core;
using RecommendationsGatewayModule.Core.Configuration;
using RecommendationsGatewayModule.Core.Requests;
using RecommendationsGatewayModule.Core.Schemas;
using RecommendationsGatewayModule.Data;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

namespace RecommendationsGatewayModule.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IContentRenderer, LiquidContentRenderer>();
            serviceCollection.AddMediatR(typeof(GetRecommendationsRequestHandler));
            AbstractTypeFactory<DownstreamResponse>.RegisterType<GetRecommendationsResponse>();
            serviceCollection.AddSchemaBuilder<ProductRecommendationSchema>();
            serviceCollection.AddSchemaType<ProductRecommendationType>();

            //PT-1611: ValidateDataAnnotations() doesn't throws any exception on the first access of IOptions<RecommendationOptions>.Value.
            //Need to investigate why this doesn't work as expected because it makes difficult to diagnose errors in the configuration.
            serviceCollection.AddOptions<RecommendationOptions>().Bind(Configuration.GetSection("Recommendations"))
                    .ValidateDataAnnotations().PostConfigure(opts =>
                                                    {
                                                        //match connection for scenario
                                                        foreach (var scenario in opts.Scenarios)
                                                        {
                                                            scenario.Connection = opts.Connections.FirstOrDefault(x => x.Name.EqualsInvariant(scenario.ConnectionName));
                                                        }
                                                    });
            serviceCollection.AddHttpClient<IDownstreamRequestSender, DownstreamRequestSender>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
        }

        public void Uninstall()
        {
        }
    }
}

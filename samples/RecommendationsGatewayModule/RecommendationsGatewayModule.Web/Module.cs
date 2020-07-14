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
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection services)
        {
            services.AddSingleton<IContentRenderer, LiquidContentRenderer>();
            services.AddMediatR(typeof(GetRecommendationsRequestHandler));
            AbstractTypeFactory<DownstreamResponse>.RegisterType<GetRecommendationsResponse>();
            services.AddSchemaBuilder<ProductRecommendationSchema>();
            services.AddSchemaType<ProductRecommendationType>();

            // Build an intermediate service provider
            var sp = services.BuildServiceProvider();
            // Resolve the services from the service provider
            var configuration = sp.GetService<IConfiguration>();
            //TODO: ValidateDataAnnotations() doesn't throws any exception on the first access of IOptions<RecommendationOptions>.Value.
            //Need to investigate why this doesn't work as expected because it makes difficult to diagnose errors in the configuration.
            services.AddOptions<RecommendationOptions>().Bind(configuration.GetSection("Recommendations"))
                    .ValidateDataAnnotations().PostConfigure(opts =>
                                                    {
                                                        //match connection for scenario
                                                        foreach (var scenario in opts.Scenarios)
                                                        {
                                                            scenario.Connection = opts.Connections.FirstOrDefault(x => x.Name.EqualsInvariant(scenario.ConnectionName));
                                                        }
                                                    });
            services.AddHttpClient<IDownstreamRequestSender, DownstreamRequestSender>()
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


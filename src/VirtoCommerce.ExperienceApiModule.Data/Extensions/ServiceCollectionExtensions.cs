using GraphQL.Introspection;
using GraphQL.Server;
using GraphQL.Types;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Data.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Data.Services;
using VirtoCommerce.Platform.Security.Services;
using ContactSignInValidator = VirtoCommerce.ExperienceApiModule.Data.Services.Security.ContactSignInValidator;
using DynamicPropertyResolverService = VirtoCommerce.ExperienceApiModule.Data.Services.DynamicPropertyResolverService;
using DynamicPropertyUpdaterService = VirtoCommerce.ExperienceApiModule.Data.Services.DynamicPropertyUpdaterService;
using IGraphQLBuilder = GraphQL.Server.IGraphQLBuilder;
using UserManagerCore = VirtoCommerce.ExperienceApiModule.Data.Services.UserManagerCore;

namespace VirtoCommerce.ExperienceApiModule.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedLockService(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("RedisConnectionString");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddSingleton<IDistributedLockService, DistributedLockService>();
            }
            else
            {
                services.AddSingleton<IDistributedLockService, NoLockService>();
            }

            return services;
        }

        public static IServiceCollection AddXCore(this IServiceCollection services, IGraphQLBuilder graphQlbuilder, IConfiguration configuration)
        {
            var assemblyMarkerCore = typeof(XCoreAnchor);
            var assemblyMarkerData = typeof(XCoreDataAnchor);
            graphQlbuilder.AddGraphTypes(assemblyMarkerCore);
            services.AddMediatR(assemblyMarkerCore, assemblyMarkerData);
            services.AddAutoMapper(assemblyMarkerCore, assemblyMarkerData);
            services.AddSchemaBuilders(assemblyMarkerData);

            //Register custom GraphQL dependencies
            services.AddPermissionAuthorization();

            services.AddSingleton<ISchemaFilter, CustomSchemaFilter>();
            services.AddSingleton<ISchema, SchemaFactory>();

            services.AddTransient<IDynamicPropertyResolverService, DynamicPropertyResolverService>();
            services.AddTransient<IDynamicPropertyUpdaterService, DynamicPropertyUpdaterService>();
            services.AddTransient<IUserManagerCore, UserManagerCore>();
            services.AddTransient<IUserSignInValidator, ContactSignInValidator>();

            // provider for external fields
            services.AddSingleton<IExternalFieldProvider, ExternalFieldProvider>();

            services.AddTransient<ILoadUserToEvalContextService, LoadUserToEvalContextService>();
            services.AddDistributedLockService(configuration);

            return services;
        }
    }
}

using System;
using System.Linq;
using System.Reflection;
using GraphQL.Authorization;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Internal;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ISchemaTypeBuilder<TSchemaType> AddSchemaType<TSchemaType>(this IServiceCollection services) where TSchemaType : class, IGraphType
        {
            //Register GraphQL Schema type in the ServicesCollection
            services.AddSingleton<TSchemaType>();

            return new SchemaTypeBuilder<TSchemaType>(services);
        }

        public static void AddPermissionAuthorization(this IServiceCollection services)
        {
            services.TryAddSingleton<IAuthorizationEvaluator, PermissionAuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();
        }

        public static void AddSchemaBuilder<TSchemaBuilder>(this IServiceCollection services) where TSchemaBuilder : class, ISchemaBuilder
        {
            services.AddSingleton<ISchemaBuilder, TSchemaBuilder>();
        }

        public static void AddGraphShemaBuilders(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            => services.AddGraphShemaBuilders(Assembly.GetCallingAssembly(), serviceLifetime);

        public static void AddGraphShemaBuilders(this IServiceCollection services, Type typeFromAssembly, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
           => services.AddGraphShemaBuilders(typeFromAssembly.Assembly, serviceLifetime);

        public static void AddGraphShemaBuilders(this IServiceCollection services, Assembly assembly, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            //Dynamic schema building
            services.AddSingleton<ISchema, SchemaFactory>();

            // Register all GraphQL types
            foreach (var type in assembly.GetTypes()
                .Where(x => !x.IsAbstract && typeof(ISchemaBuilder).IsAssignableFrom(x)))
            {
                services.TryAdd(new ServiceDescriptor(typeof(ISchemaBuilder), type, serviceLifetime));
            }
        }

        public static void AddServiceGateways<T>(this IServiceCollection serviceCollection, Type[] implemtationTypes) where T : IServiceGateway
        {
            foreach (var implType in implemtationTypes)
            {
                serviceCollection.AddTransient(typeof(IServiceGateway), implType);
            }

            serviceCollection.AddTransient(typeof(T), factory =>
            {
                var providers = factory.GetServices<IServiceGateway>();
                var config = factory.GetService<IOptions<ExperienceOptions>>().Value;
                return providers.FirstOrDefault(p => p.GetType().GetInterfaces().Contains(typeof(T)) && p.Gateway.EqualsInvariant(config.Gateway));
            });
        }
    }
}

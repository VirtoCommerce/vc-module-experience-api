using System;
using System.Linq;
using System.Reflection;
using GraphQL.Authorization;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Internal;
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

        public static void AddSchemaBuilders(this IServiceCollection services)
        {
            services.AddSchemaBuilders(Assembly.GetCallingAssembly());
        }

        public static void AddSchemaBuilders(this IServiceCollection services, Type assemblyMarkerType)
        {
            services.AddSchemaBuilders(assemblyMarkerType.Assembly);
        }

        public static void AddSchemaBuilders(this IServiceCollection services, Assembly assembly)
        {
            var schemaBuilder = typeof(ISchemaBuilder);

            foreach (var type in assembly.GetTypes().Where(x => !x.IsAbstract && x.IsAssignableTo(schemaBuilder)))
            {
                services.TryAddEnumerable(new ServiceDescriptor(schemaBuilder, type, ServiceLifetime.Singleton));
            }
        }

        public static void OverrideSchemaBuilder<TOld, TNew>(this IServiceCollection serviceCollection)
            where TOld : class, ISchemaBuilder
            where TNew : class, ISchemaBuilder, TOld
        {
            var descriptor = serviceCollection.FirstOrDefault(x =>
                x.ServiceType == typeof(ISchemaBuilder) &&
                x.ImplementationType == typeof(TOld));

            if (descriptor != null)
            {
                serviceCollection.Remove(descriptor);
                serviceCollection.AddSchemaBuilder<TNew>();
            }
        }

        public static void OverrideGraphType<TOld, TNew>(this IServiceCollection services)
            where TOld : class, IGraphType
            where TNew : class, IGraphType, TOld
        {
            services.AddSingleton<TNew>();
            AbstractTypeFactory<IGraphType>.OverrideType<TOld, TNew>();
        }
    }
}

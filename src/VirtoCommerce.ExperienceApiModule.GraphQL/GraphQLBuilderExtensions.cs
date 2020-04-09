using System;
using System.Linq;
using System.Reflection;
using GraphQL.Authorization;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace VirtoCommerce.ExperienceApiModule.GraphQLEx
{
    public static class GraphQLBuilderExtensions
    {
        public static IGraphQLBuilder AddPermissionAuthorization(this IGraphQLBuilder builder)
        {
            builder.Services.TryAddSingleton<IAuthorizationEvaluator, PermissionAuthorizationEvaluator>();
            builder.Services.AddTransient<IValidationRule, AuthorizationValidationRule>();
            return builder;
        }
        
        public static IGraphQLBuilder AddGraphShemaBuilders(
            this IGraphQLBuilder builder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            => builder.AddGraphShemaBuilders(Assembly.GetCallingAssembly(), serviceLifetime);

        
        public static IGraphQLBuilder AddGraphShemaBuilders(
            this IGraphQLBuilder builder,
            Type typeFromAssembly,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            => builder.AddGraphShemaBuilders(typeFromAssembly.Assembly, serviceLifetime);

     
        public static IGraphQLBuilder AddGraphShemaBuilders(
            this IGraphQLBuilder builder,
            Assembly assembly,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            //Dynamic schema building
            builder.Services.AddSingleton<ISchema, SchemaFactory>();

            // Register all GraphQL types
            foreach (var type in assembly.GetTypes()
                .Where(x => !x.IsAbstract && typeof(ISchemaBuilder).IsAssignableFrom(x)))
            {
                builder.Services.TryAdd(new ServiceDescriptor(typeof(ISchemaBuilder), type, serviceLifetime));
            }

            return builder;
        }

    }
}

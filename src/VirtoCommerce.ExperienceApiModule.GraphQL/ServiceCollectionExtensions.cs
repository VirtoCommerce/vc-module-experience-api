using System;
using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Relay.Types;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Types.Relay;
using GraphQL.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace VirtoCommerce.ExperienceApiModule.GraphQLEx
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGraphQLServer(this IServiceCollection services)
        {
            // Register these relay types
            services.AddTransient(typeof(ConnectionType<>));
            services.AddTransient(typeof(EdgeType<>));
            services.AddTransient<NodeInterface>();
            services.AddTransient<PageInfoType>();

            //Dynamic schema building
            services.AddSingleton<ISchema, SchemaFactory>();

            // And some stuff to make GraphQL work
            services.TryAddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.TryAddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>();

            //Register .NET GraphQL server
            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
            }).AddNewtonsoftJson(deserializerSettings => { }, serializerSettings => { })
            .AddUserContextBuilder(context => new GraphQLUserContext { User = context.User });

            //Add auth
            services.AddGraphQLAuth();
        }

        public static void AddGraphQLAuth(this IServiceCollection services)
        {
            services.TryAddSingleton<IAuthorizationEvaluator, PermissionAuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();
        }
    }

}

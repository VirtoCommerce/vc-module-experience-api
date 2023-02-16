using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class FieldTypeExtenstions
    {
        public static FieldBuilder<TSourceType, TReturnType> Argument<TSourceType, TReturnType>(this FieldBuilder<TSourceType, TReturnType> fieldBuilder, Type type, string name)
        {
            var arg = new QueryArgument(type)
            {
                Name = name,
            };

            fieldBuilder.FieldType.Arguments.Add(arg);

            return fieldBuilder;
        }
    }

    public static class FieldTypeExtensions
    {
        public static FieldBuilder<TSourceType, TReturnType> ResolveSynchronized<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> fieldBuilder,
            string resourceKeyPrefix,
            string resourceKeyProperty,
            IDistributedLockService distributedLockService,
            Func<IResolveFieldContext<TSourceType>, TReturnType> resolve)
        {
            TReturnType ResolveWrapper(IResolveFieldContext<TSourceType> context)
            {
                // Find resource key in context
                var resourceKey = GetResourceKey(context, resourceKeyPrefix, resourceKeyProperty);

                return string.IsNullOrEmpty(resourceKey)
                    ? resolve(context)
                    : distributedLockService.Execute(resourceKey, () => resolve(context));
            }

            fieldBuilder.FieldType.Resolver = new FuncFieldResolver<TSourceType, TReturnType>(ResolveWrapper);

            return fieldBuilder;
        }

        public static FieldBuilder<TSourceType, TReturnType> ResolveSynchronizedAsync<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> fieldBuilder,
            string resourceKeyPrefix,
            string resourceKeyProperty,
            IDistributedLockService distributedLockService,
            Func<IResolveFieldContext<TSourceType>, Task<TReturnType>> resolve)
        {
            async Task<TReturnType> ResolveWrapperAsync(IResolveFieldContext<TSourceType> context)
            {
                // Find resource key in context
                var resourceKey = GetResourceKey(context, resourceKeyPrefix, resourceKeyProperty);

                return string.IsNullOrEmpty(resourceKey)
                    ? await resolve(context)
                    : await distributedLockService.ExecuteAsync(resourceKey, async () => await resolve(context));
            }

            fieldBuilder.FieldType.Resolver = new AsyncFieldResolver<TSourceType, TReturnType>(ResolveWrapperAsync);

            return fieldBuilder;
        }


        private static string GetResourceKey<TSourceType>(IResolveFieldContext<TSourceType> context, string resourceKeyPrefix, string resourceKeyProperty)
        {
            var command = context.GetArgument<IDictionary<string, object>>("command");

            if (command == null ||
                !command.TryGetValue(resourceKeyProperty, out var value) ||
                value is not string resourceKey ||
                string.IsNullOrEmpty(resourceKey))
            {
                return null;
            }

            return $"{resourceKeyPrefix}:{resourceKey}";
        }
    }
}

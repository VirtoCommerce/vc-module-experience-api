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

        public static FieldBuilder<TSourceType, TReturnType> ResolveSyncronized<TSourceType, TReturnType>(this FieldBuilder<TSourceType, TReturnType> fieldBuilder,
            string resourceKeyProperty,
            IDistributedLockService distributedLockService,
            Func<IResolveFieldContext<TSourceType>, TReturnType> resolve)
        {
            Func<IResolveFieldContext<TSourceType>, TReturnType> resolveWrapper = (context) =>
            {
                var id = context.GetCurrentUserId();

                return resolve(context);
            };

            var resolver = new FuncFieldResolver<TSourceType, TReturnType>(resolveWrapper);
            fieldBuilder.FieldType.Resolver = resolver;

            return fieldBuilder;
        }

        public static FieldBuilder<TSourceType, TReturnType> ResolveSyncronizedAsync<TSourceType, TReturnType>(this FieldBuilder<TSourceType, TReturnType> fieldBuilder,
            string resourceKeyProperty,
            IDistributedLockService distributedLockService,
            Func<IResolveFieldContext<TSourceType>, Task<TReturnType>> resolve)
        {
            Func<IResolveFieldContext<TSourceType>, Task<TReturnType>> resolveWrapper = async (context) =>
            {
                //find resource key in context
                var resourceKey = GetResourceKey(context, resourceKeyProperty);

                if (!string.IsNullOrEmpty(resourceKey))
                {
                    return await distributedLockService.ExecuteAsync(resourceKey, async () =>
                    {
                        return await resolve(context);
                    });
                }
                else
                {
                    return await resolve(context);
                }
            };

            var resolver = new AsyncFieldResolver<TSourceType, TReturnType>(resolveWrapper);
            fieldBuilder.FieldType.Resolver = resolver;

            return fieldBuilder;
        }

        private static string GetResourceKey<TSourceType>(IResolveFieldContext<TSourceType> context, string resourceKeyProperty)
        {
            var command = context.GetArgument<IDictionary<string, object>>("command");
            if (command == null)
            {
                return null;
            }

            _ = command.TryGetValue(resourceKeyProperty, out var value);
            return value as string;
        }
    }
}

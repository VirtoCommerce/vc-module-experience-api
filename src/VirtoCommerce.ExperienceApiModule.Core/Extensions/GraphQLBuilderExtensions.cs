using System;
using GraphQL.Server;
using GraphQL.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class GraphQLBuilderExtensions
    {
        public static IGraphQLBuilder AddCustomValidationRule<TRule>(this IGraphQLBuilder builder) where TRule : class, IValidationRule
        {
            builder.Services.AddTransient<IValidationRule, TRule>();

            return builder;
        }

        public static IGraphQLBuilder AddCustomValidationRule(this IGraphQLBuilder builder, Type ruleType)
        {
            builder.Services.AddTransient(typeof(IValidationRule), ruleType);

            return builder;
        }
    }
}

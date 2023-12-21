using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Server;
using GraphQL.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class GraphQLBuilderExtensions
    {
        public static IGraphQLBuilder ReplaceValidationRule<TOldRule, TNewRule>(this IGraphQLBuilder builder)
            where TOldRule : class, IValidationRule
            where TNewRule : class, IValidationRule
        {
            var coreRules = DocumentValidator.CoreRules as List<IValidationRule>;
            var oldRuleType = typeof(TOldRule);
            var oldRule = coreRules?.FirstOrDefault(x => x.GetType() == oldRuleType);

            if (oldRule != null)
            {
                coreRules.Remove(oldRule);
                builder.AddCustomValidationRule<TNewRule>();
            }

            return builder;
        }

        public static IGraphQLBuilder AddCustomValidationRule<TRule>(this IGraphQLBuilder builder)
            where TRule : class, IValidationRule
        {
            builder.Services.AddSingleton<IValidationRule, TRule>();

            return builder;
        }

        public static IGraphQLBuilder AddCustomValidationRule(this IGraphQLBuilder builder, Type ruleType)
        {
            builder.Services.AddSingleton(typeof(IValidationRule), ruleType);

            return builder;
        }
    }
}

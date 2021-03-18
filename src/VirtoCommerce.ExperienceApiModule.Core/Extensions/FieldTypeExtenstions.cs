using System;
using GraphQL.Builders;
using GraphQL.Types;

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
}

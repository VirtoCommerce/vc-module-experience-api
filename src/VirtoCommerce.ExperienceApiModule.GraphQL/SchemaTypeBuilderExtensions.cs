using GraphQL.Types;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.GraphQLEx
{
    public static class SchemaTypeBuilderExtensions
    {
        public static ISchemaTypeBuilder<TDerivedSchemaType> OverrideType<TBaseSchemaType, TDerivedSchemaType>(this ISchemaTypeBuilder<TDerivedSchemaType> builder) where TBaseSchemaType : IGraphType where TDerivedSchemaType : TBaseSchemaType, IGraphType
        {
            AbstractTypeFactory<IGraphType>.OverrideType<TBaseSchemaType, TDerivedSchemaType>();
            return builder;
        }
    }
}

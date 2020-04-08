using System;
using GraphQL.Types;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.GraphQLEx
{
    public static class GraphTypeExtenstionHelper
    {
        public static void OverrideGraphType<TBaseType, TDerivedType>() where TDerivedType : IGraphType
        {
            AbstractTypeFactory<IGraphType>.OverrideType<TBaseType, TDerivedType>();
        }
        //Returns the actual (overridden) type for requested 
        public static Type GetActualType<TGraphType>() where TGraphType : IGraphType
        {
            var graphType = typeof(TGraphType);
            var result = AbstractTypeFactory<IGraphType>.FindTypeInfoByName(graphType.Name)?.Type;
            if (result == null)
            {
                result = graphType;
            }
            return result;
        }
    }
}

using System;
using System.Linq;
using GraphQL.Builders;
using GraphQL.Types;
using GraphQL.Types.Relay;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Helpers
{
    public static class GraphTypeExtenstionHelper
    {
        //Returns the actual (overridden) type for requested
        public static Type GetActualType<TGraphType>() where TGraphType : IGraphType
        {
            var graphType = typeof(TGraphType);

            return GetActualType(graphType);
        }

        public static Type GetActualType(Type graphType)
        {
            var result = AbstractTypeFactory<IGraphType>.FindTypeInfoByName(graphType.Name)?.Type;

            if (result == null)
            {
                result = graphType;
            }

            return result;
        }

        /// For generic graph type definitions like NonNullGraphType ProdcutType
        /// or NonNullGraphType ListGraphType ProdcutType
        public static Type GetComplexType<TGraphType>() where TGraphType : IGraphType
        {
            var outerGraphType = typeof(TGraphType);

            return GetComplexTypeRecursive(outerGraphType);
        }

        private static Type GetComplexTypeRecursive(Type outerGraphType)
        {
            Type complexType;

            if (outerGraphType.IsGenericType && outerGraphType.GenericTypeArguments.Length > 0)
            {
                var actualInnerType = GetComplexTypeRecursive(outerGraphType.GenericTypeArguments[0]);

                complexType = outerGraphType.GetGenericTypeDefinition().MakeGenericType(new[] { actualInnerType });
            }
            else
            {
                complexType = GetActualType(outerGraphType);
            }

            return complexType;
        }

        public static ConnectionBuilder<TSourceType> CreateConnection<TNodeType, TSourceType>() where TNodeType : IGraphType
        {
            return CreateConnection<TNodeType, EdgeType<TNodeType>, ConnectionType<TNodeType>, TSourceType>();
        }

        public static ConnectionBuilder<TSourceType> CreateConnection<TNodeType, TEdgeType, TConnectionType, TSourceType>()
            where TNodeType : IGraphType
            where TEdgeType : EdgeType<TNodeType>
            where TConnectionType : ConnectionType<TNodeType, TEdgeType>
        {
            //EdgeType<ProductType>, ProductsConnectonType<ProductType>
            //Try first find the actual TNodeType  in the  AbstractTypeFactory
            var actualNodeType = GetActualType<TNodeType>();
            var createMethodInfo = typeof(ConnectionBuilder<>).MakeGenericType(typeof(TSourceType)).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant(nameof(ConnectionBuilder.Create)) && x.GetGenericArguments().Count() == 3);
            var genericEgdeType = typeof(TEdgeType).GetGenericTypeDefinition().MakeGenericType(new[] { actualNodeType });
            var genericConnectionType = typeof(TConnectionType).GetGenericTypeDefinition().MakeGenericType(new[] { actualNodeType });
            var connectionBuilder = (ConnectionBuilder<TSourceType>)createMethodInfo.MakeGenericMethod(actualNodeType, genericEgdeType, genericConnectionType).Invoke(null, new[] { Type.Missing });
            return connectionBuilder;
        }
    }
}

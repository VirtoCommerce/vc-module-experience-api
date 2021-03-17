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

        /// <summary>
        /// For generic graph type definitions like NonNullGraphType ProdcutType
        /// or NonNullGraphType ListGraphType ProdcutType
        /// </summary>
        public static Type GetActualComplexType<TGraphType>() where TGraphType : IGraphType
        {
            var outerGraphType = typeof(TGraphType);

            return GetActualComplexTypeRecursive(outerGraphType);
        }

        private static Type GetActualComplexTypeRecursive(Type outerGraphType)
        {
            if (outerGraphType.IsGenericType && outerGraphType.GenericTypeArguments.Length > 0)
            {
                var actualInnerType = GetActualComplexTypeRecursive(outerGraphType.GenericTypeArguments[0]);

                return outerGraphType.GetGenericTypeDefinition().MakeGenericType(new[] { actualInnerType });
            }

            return GetActualType(outerGraphType);
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

using System;
using System.Linq;
using GraphQL.Builders;
using GraphQL.Types;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.GraphQLEx
{
    public static class ConnectionBuilderExt
    {
        public static ConnectionBuilder<TSourceType> Create<TNodeType, TSourceType>() where TNodeType : IGraphType
        {
            //Try first find the actual TNodeType  in the  AbstractTypeFactory
            var actualNodeType = GraphTypeExtenstionHelper.GetActualType<TNodeType>();
            var createMethodInfo = typeof(ConnectionBuilder<>).MakeGenericType(typeof(TSourceType)).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant(nameof(ConnectionBuilder.Create)) && x.GetGenericArguments().Count() == 1);
            var connectionBuilder = (ConnectionBuilder<TSourceType>)createMethodInfo.MakeGenericMethod(actualNodeType).Invoke(null, new[] { Type.Missing });
            return connectionBuilder;
        }
    }
}

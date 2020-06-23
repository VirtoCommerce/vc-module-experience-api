using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.XPurchase.Domain.Factories;
using VirtoCommerce.XPurchase.Interfaces;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class SchemaBuilderExtensions
    {
        public static ISchema RegisterQueryType<T>(this ISchema schema, IShoppingCartAggregateFactory cartFactory)
            where T : class, ISimpleQueryType, new()
        {
            var queryBuilder = new T();

            var query = queryBuilder.GetQueryType(cartFactory);

            schema.Query.AddField(query);

            return schema;
        }

        public static ISchema RegisterQueryType<T>(this ISchema schema,
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader,
            IShoppingCartAggregateFactory cartFactory)
            where T : class, ISmartQueryType, new()
        {
            var queryBuilder = new T();

            var query = queryBuilder.GetQueryType(mediator, dataLoader, cartFactory);

            schema.Query.AddField(query);

            return schema;
        }

        public static ISchema RegisterMutationType<T>(this ISchema schema,
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
            where T : class, IBuildableMutation, new()
        {
            var mutationBuilder = new T();

            var mutation = mutationBuilder.GetMutationType(mediator, dataLoader);

            schema.Mutation.AddField(mutation);

            return schema;
        }
    }
}

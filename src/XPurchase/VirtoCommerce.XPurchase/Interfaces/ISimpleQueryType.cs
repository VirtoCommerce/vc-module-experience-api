using GraphQL.Types;
using VirtoCommerce.XPurchase.Domain.Factories;

namespace VirtoCommerce.XPurchase.Interfaces
{
    public interface ISimpleQueryType : IBuildableQuery
    {
        FieldType GetQueryType(IShoppingCartAggregateFactory cartFactory);
    }
}

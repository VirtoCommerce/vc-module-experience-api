using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.XPurchase.Domain.Factories;

namespace VirtoCommerce.XPurchase.Interfaces
{
    public interface ISmartQueryType : IBuildableQuery
    {
        FieldType GetQuery(IMediator mediator, IDataLoaderContextAccessor dataLoader, IShoppingCartAggregateFactory cartFactory);
    }
}

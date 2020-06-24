using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;

namespace VirtoCommerce.XPurchase.Interfaces
{
    public interface IBuildableMutation
    {
        FieldType GetMutationType(IMediator mediator, IDataLoaderContextAccessor dataLoader);
    }
}

using GraphQL.DataLoader;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Services;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartGiftItemType : LineItemType
    {
        public CartGiftItemType(IMediator mediator, IDataLoaderContextAccessor dataLoader, IDynamicPropertyResolverService dynamicPropertyResolverService) : base(mediator, dataLoader, dynamicPropertyResolverService)
        {
            Field(x => x.IsRejected).Description("Was the line item rejected");
        }
    }
}

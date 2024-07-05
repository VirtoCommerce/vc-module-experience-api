using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Commands;

public abstract class ScopedWishlistCommandHandlerBase<TCommand> : CartCommandHandler<TCommand>
    where TCommand : ScopedWishlistCommand
{
    protected ScopedWishlistCommandHandlerBase(ICartAggregateRepository cartAggregateRepository)
        : base(cartAggregateRepository)
    {
    }

    protected virtual Task UpdateScopeAsync(CartAggregate cartAggregate, TCommand request)
    {
        if (request.Scope?.EqualsIgnoreCase(XPurchaseConstants.OrganizationScope) == true)
        {
            if (!string.IsNullOrEmpty(request.WishlistUserContext.CurrentOrganizationId))
            {
                cartAggregate.Cart.OrganizationId = request.WishlistUserContext.CurrentOrganizationId;
            }
        }
        else if (request.Scope?.EqualsIgnoreCase(XPurchaseConstants.PrivateScope) == true)
        {
            cartAggregate.Cart.OrganizationId = null;

            cartAggregate.Cart.CustomerId = request.WishlistUserContext.CurrentUserId;
            cartAggregate.Cart.CustomerName = request.WishlistUserContext.CurrentContact.Name;
        }

        return Task.CompletedTask;
    }
}

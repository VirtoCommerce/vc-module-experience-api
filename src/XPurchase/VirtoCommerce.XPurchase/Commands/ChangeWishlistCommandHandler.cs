using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeWishlistCommandHandler : ScopedWishlistCommandHandlerBase<ChangeWishlistCommand>
    {
        public ChangeWishlistCommandHandler(ICartAggregateRepository cartAggregateRepository)
            : base(cartAggregateRepository)
        {
        }

        public override async Task<CartAggregate> Handle(ChangeWishlistCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = request.WishlistUserContext.Cart == null
                ? await CartRepository.GetCartByIdAsync(request.ListId, request.CultureName)
                : await CartRepository.GetCartForShoppingCartAsync(request.WishlistUserContext.Cart, request.CultureName);

            if (request.ListName != null)
            {
                cartAggregate.Cart.Name = request.ListName;
            }

            if (request.Description != null)
            {
                cartAggregate.Cart.Description = request.Description;
            }

            await UpdateScopeAsync(cartAggregate, request);

            return await SaveCartAsync(cartAggregate);
        }


        [Obsolete("Use UpdateScopeAsync()", DiagnosticId = "VC0009", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        protected virtual Task ChangeScope(ChangeWishlistCommand request, CartAggregate cartAggregate)
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
}

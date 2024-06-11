using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class ChangeWishlistCommandHandler : CartCommandHandler<ChangeWishlistCommand>
    {
        private readonly IMemberResolver _memberResolver;

        public ChangeWishlistCommandHandler(ICartAggregateRepository cartAggrRepository, IMemberResolver memberResolver)
            : base(cartAggrRepository)
        {
            _memberResolver = memberResolver;
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

            await ChangeScope(request, cartAggregate);

            return await SaveCartAsync(cartAggregate);
        }

        protected virtual async Task ChangeScope(ChangeWishlistCommand request, CartAggregate cartAggregate)
        {
            var contact = request.WishlistUserContext.CurrentContact ?? await _memberResolver.ResolveMemberByIdAsync(request.UserId) as Contact;

            if (request.Scope?.EqualsInvariant(ModuleConstants.OrganizationScope) == true)
            {
                cartAggregate.Cart.OrganizationId = contact?.Organizations?.FirstOrDefault();
            }
            else if (request.Scope?.EqualsInvariant(ModuleConstants.PrivateScope) == true)
            {
                cartAggregate.Cart.OrganizationId = null;
                cartAggregate.Cart.CustomerId = request.WishlistUserContext.CurrentUserId;
                cartAggregate.Cart.CustomerName = contact?.Name;
            }
        }
    }
}

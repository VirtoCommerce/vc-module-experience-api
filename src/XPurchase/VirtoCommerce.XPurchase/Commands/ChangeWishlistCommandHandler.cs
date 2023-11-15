using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Commands
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
            var cartAggregate = request.Cart == null
                ? await CartRepository.GetCartByIdAsync(request.ListId)
                : await CartRepository.GetCartForShoppingCartAsync(request.Cart);

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
            var contact = request.Contact ?? await _memberResolver.ResolveMemberByIdAsync(request.UserId) as Contact;

            if (request.Scope?.EqualsInvariant(XPurchaseConstants.OrganizationScope) == true)
            {
                var organizationId = contact?.Organizations?.FirstOrDefault();

                cartAggregate.Cart.OrganizationId = organizationId;
            }
            else if (request.Scope?.EqualsInvariant(XPurchaseConstants.PrivateScope) == true)
            {
                cartAggregate.Cart.OrganizationId = null;
                cartAggregate.Cart.CustomerId = request.UserId;
                cartAggregate.Cart.CustomerName = contact?.Name;
            }
        }
    }
}

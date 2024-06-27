using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateWishlistCommandHandler : CartCommandHandler<CreateWishlistCommand>
    {
        private readonly IMemberResolver _memberResolver;

        public CreateWishlistCommandHandler(ICartAggregateRepository cartAggrRepository, IMemberResolver memberResolver)
            : base(cartAggrRepository)
        {
            _memberResolver = memberResolver;
        }

        public override async Task<CartAggregate> Handle(CreateWishlistCommand request, CancellationToken cancellationToken)
        {
            request.CartType = XPurchaseConstants.ListTypeName;

            var cartAggregate = await CreateNewCartAggregateAsync(request);

            var contact = await _memberResolver.ResolveMemberByIdAsync(request.UserId) as Contact;

            cartAggregate.Cart.Description = request.Description;
            cartAggregate.Cart.CustomerName = contact?.Name;

            if (request.Scope?.EqualsInvariant(XPurchaseConstants.OrganizationScope) == true)
            {
                cartAggregate.Cart.OrganizationId = request.OrganizationId;
            }

            return await SaveCartAsync(cartAggregate);
        }
    }
}

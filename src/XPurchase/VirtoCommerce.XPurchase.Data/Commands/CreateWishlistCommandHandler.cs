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
            request.CartType = ModuleConstants.ListTypeName;

            var cartAggregate = await CreateNewCartAggregateAsync(request);

            var contact = await _memberResolver.ResolveMemberByIdAsync(request.UserId) as Contact;

            cartAggregate.Cart.Description = request.Description;
            cartAggregate.Cart.CustomerName = contact?.Name;

            if (request.Scope?.EqualsInvariant(ModuleConstants.OrganizationScope) == true)
            {
                var organizationId = contact?.Organizations?.FirstOrDefault();

                cartAggregate.Cart.OrganizationId = organizationId;
            }

            return await SaveCartAsync(cartAggregate);
        }
    }
}

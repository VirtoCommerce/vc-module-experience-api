using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Commands;

public class CloneWishlistCommandHandler : CartCommandHandler<CloneWishlistCommand>
{
    private readonly IMemberResolver _memberResolver;

    public CloneWishlistCommandHandler(
        ICartAggregateRepository cartAggregateRepository,
        IMemberResolver memberResolver)
        : base(cartAggregateRepository)
    {
        _memberResolver = memberResolver;
    }

    public override async Task<CartAggregate> Handle(CloneWishlistCommand request, CancellationToken cancellationToken)
    {
        request.CartType = XPurchaseConstants.ListTypeName;
        var cloneCartAggregate = await CreateNewCartAggregateAsync(request);
        cloneCartAggregate.Cart.Description = request.Description;

        if (request.Scope?.EqualsInvariant(XPurchaseConstants.OrganizationScope) == true)
        {
            var contact = await _memberResolver.ResolveMemberByIdAsync(request.UserId) as Contact;
            
            var organizationId = contact?.Organizations?.FirstOrDefault();

            cloneCartAggregate.Cart.OrganizationId = organizationId;
        }

        var cartAggregate = await GetCartById(request.ListId, request.CultureName);

        if (cartAggregate != null)
        {
            await cloneCartAggregate.AddItemsAsync(cartAggregate.Cart?.Items?.Select(x =>
                new NewCartItem(x.ProductId, x.Quantity) { IsWishlist = true }).ToArray());
        }

        return await SaveCartAsync(cloneCartAggregate);
    }
}

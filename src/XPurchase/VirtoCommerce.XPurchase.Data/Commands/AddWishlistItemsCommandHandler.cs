using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class AddWishlistItemsCommandHandler : CartCommandHandler<AddWishlistItemsCommand>
    {
        public AddWishlistItemsCommandHandler(ICartAggregateRepository cartAggregateRepository)
            : base(cartAggregateRepository)
        {
        }

        public override async Task<CartAggregate> Handle(AddWishlistItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);

            cartAggregate.ValidationRuleSet = new string[] { "default" };

            foreach (var listItem in request.ListItems)
            {
                listItem.IsWishlist = true;
            }

            await cartAggregate.AddItemsAsync(request.ListItems);

            return await SaveCartAsync(cartAggregate);
        }
    }
}

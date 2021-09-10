using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddGiftItemsCommandHandler : CartCommandHandler<AddGiftItemsCommand>
    {
        private readonly ICartAvailMethodsService _cartAvailMethodsService;


        public AddGiftItemsCommandHandler(ICartAggregateRepository cartRepository, ICartAvailMethodsService cartAvailMethodsService)
            : base(cartRepository)
        {
            _cartAvailMethodsService = cartAvailMethodsService;
        }

        public override async Task<CartAggregate> Handle(AddGiftItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.AddGiftItemsAsync(request.GiftItemIds, (await _cartAvailMethodsService.GetAvailableGiftsAsync(cartAggregate)).ToList());

            return await SaveCartAsync(cartAggregate);
        }
    }
}

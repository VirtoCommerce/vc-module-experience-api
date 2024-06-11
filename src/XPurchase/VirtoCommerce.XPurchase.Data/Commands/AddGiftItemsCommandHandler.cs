using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
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

            await cartAggregate.AddGiftItemsAsync(request.Ids, (await _cartAvailMethodsService.GetAvailableGiftsAsync(cartAggregate)).ToList());

            return await SaveCartAsync(cartAggregate);
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddWishlistBulkItemCommandHandler : IRequestHandler<AddWishlistBulkItemCommand, BulkCartAggregateResult>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;

        public AddWishlistBulkItemCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            _cartAggregateRepository = cartAggrRepository;
        }

        public async Task<BulkCartAggregateResult> Handle(AddWishlistBulkItemCommand request, CancellationToken cancellationToken)
        {
            var result = new BulkCartAggregateResult();

            foreach (var listId in request.ListIds)
            {
                var cartAggregate = await _cartAggregateRepository.GetCartByIdAsync(listId);

                await cartAggregate.AddItemsAsync(new List<NewCartItem> {
                    new NewCartItem(request.ProductId, request.Quantity ?? 1)
                });

                await _cartAggregateRepository.SaveAsync(cartAggregate);

                result.CartAggregates.Add(cartAggregate);
            }

            return result;
        }
    }
}

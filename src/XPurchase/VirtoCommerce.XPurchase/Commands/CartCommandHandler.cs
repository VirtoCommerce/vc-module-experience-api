using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.XPurchase.Domain.Aggregates;
using VirtoCommerce.XPurchase.Domain.Builders;
using VirtoCommerce.XPurchase.Domain.Factories;

namespace VirtoCommerce.XPurchase.Domain.Commands
{
    public abstract class CartCommandHandler<TCartCommand> : AsyncRequestHandler<TCartCommand> where TCartCommand : CartCommand
    {
        private readonly ICartAggregateRepository _cartAggrFactory;
        protected CartCommandHandler(ICartAggregateRepository cartAggrFactory)
        {
            _cartAggrFactory = cartAggrFactory;
        }
       
        protected Task<Aggregates.CartAggregate> GetCartAggregateFromCommandAsync(TCartCommand request)
        {
            var shoppingCartContext = CartContextBuilder.Build()
                                                            .WithStore(request.StoreId)
                                                            .WithCartName(request.CartName)
                                                            .WithUser(request.UserId)
                                                            .WithCurrencyAndLanguage(request.Currency, request.Language)
                                                            .WithCartType(request.CartType)
                                                            .GetContext();

            return _cartAggrFactory.CreateOrGetShoppingCartAggregateAsync(shoppingCartContext);
        }
    }
}

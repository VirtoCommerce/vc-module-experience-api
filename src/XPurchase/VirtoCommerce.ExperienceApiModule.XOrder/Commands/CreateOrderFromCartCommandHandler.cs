using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CartModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class CreateOrderFromCartCommandHandler : IRequestHandler<CreateOrderFromCartCommand, CustomerOrderAggregate>
    {
        private readonly IShoppingCartService _cartService;
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;

        public CreateOrderFromCartCommandHandler(IShoppingCartService cartService,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _cartService = cartService;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public virtual async Task<CustomerOrderAggregate> Handle(CreateOrderFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetByIdAsync(request.CartId);
            var result = await _customerOrderAggregateRepository.CreateOrderFromCart(cart);

            return result;
        }
    }
}

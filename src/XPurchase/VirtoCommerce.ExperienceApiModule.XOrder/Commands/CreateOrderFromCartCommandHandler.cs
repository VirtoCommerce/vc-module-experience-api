using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class CreateOrderFromCartCommandHandler : IRequestHandler<CreateOrderFromCartCommand, CustomerOrderAggregate>
    {
        private readonly ICrudService<ShoppingCart> _cartService;
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;

        public CreateOrderFromCartCommandHandler(IShoppingCartService cartService,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository)
        {
            _cartService = (ICrudService<ShoppingCart>)cartService;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
        }

        public virtual async Task<CustomerOrderAggregate> Handle(CreateOrderFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetByIdAsync(request.CartId);
            var result = await _customerOrderAggregateRepository.CreateOrderFromCart(cart);
            await _cartService.DeleteAsync(new List<string> { request.CartId }, softDelete: true);
            // Remark: There is potential bug, because there is no transaction thru two actions above. If a cart deletion fails, the order remains. That causes data inconsistency.
            // Unfortunately, current architecture does not allow us to support such scenarios in a transactional manner.
            return result;
        }
    }
}

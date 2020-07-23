using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, bool>
    {
        private readonly ICustomerOrderService _customerOrderService;
        public ChangeOrderStatusCommandHandler(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }
        public async Task<bool> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _customerOrderService.GetByIdAsync(request.OrderId);
            order.Status = request.Status;
            await _customerOrderService.SaveChangesAsync(new[] { order });
            return true;
        }
    }
}

using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XOrder.Core.Models;

namespace VirtoCommerce.XOrder.Core.Commands
{
    public class AddOrUpdateOrderPaymentCommand : ICommand<CustomerOrderAggregate>
    {
        public string OrderId { get; set; }

        public ExpOrderPayment Payment { get; set; }
    }
}

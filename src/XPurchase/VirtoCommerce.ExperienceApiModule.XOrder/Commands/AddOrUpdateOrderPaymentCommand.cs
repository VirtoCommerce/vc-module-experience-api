using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class AddOrUpdateOrderPaymentCommand : ICommand<CustomerOrderAggregate>
    {
        public string OrderId { get; set; }

        public ExpOrderPayment Payment { get; set; }
    }
}

using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class TransferOrderCommand : ICommand<CustomerOrderAggregate>
    {
        public string CustomerOrderId { get; set; }

        public string ToUserId { get; set; }

        public string UserName { get; set; }
    }
}

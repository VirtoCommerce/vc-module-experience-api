using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XOrder.Core.Commands
{
    public class ChangeOrderStatusCommand : ICommand<bool>
    {
        public ChangeOrderStatusCommand(string orderId, string status)
        {
            OrderId = orderId;
            Status = status;
        }

        public string OrderId { get; set; }
        public string Status { get; set; }
    }
}

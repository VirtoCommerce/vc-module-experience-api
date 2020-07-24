using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class CreateOrderFromCartCommand : ICommand<CustomerOrderAggregate>
    {
        public CreateOrderFromCartCommand(string cartId)
        {
            CartId = cartId;
        }

        public string CartId { get; set; }
    }
}

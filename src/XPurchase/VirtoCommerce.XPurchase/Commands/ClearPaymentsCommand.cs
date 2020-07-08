using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearPaymentsCommand : ICommand<bool>
    {
        public ClearPaymentsCommand()
        {
        }

        public ClearPaymentsCommand(string cartId)
        {
            CartId = cartId;
        }

        public string CartId { get; set; }
    }
}

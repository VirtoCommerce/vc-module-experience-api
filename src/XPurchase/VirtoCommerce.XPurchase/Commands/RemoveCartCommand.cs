using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartCommand : ICommand<bool>
    {
        public RemoveCartCommand()
        {
        }

        public RemoveCartCommand(string cartId)
        {
            CartId = cartId;
        }

        public string CartId { get; set; }
    }
}

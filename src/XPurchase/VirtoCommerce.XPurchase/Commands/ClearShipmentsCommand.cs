using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearShipmentsCommand : ICommand<bool>
    {
        public ClearShipmentsCommand()
        {
        }

        public ClearShipmentsCommand(string cartId)
        {
            CartId = cartId;
        }

        public string CartId { get; set; }
    }
}

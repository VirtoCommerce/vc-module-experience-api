using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class ChangeCartItemSelectedCommand : CartCommand
    {
        public string LineItemId { get; set; }

        public bool SelectedForCheckout { get; set; }
    }
}

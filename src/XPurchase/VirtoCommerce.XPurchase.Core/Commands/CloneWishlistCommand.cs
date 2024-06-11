using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands;

public class CloneWishlistCommand : WishlistCommand
{
    public string ListName { get => CartName; set => CartName = value; }
    public string Scope { get; set; }
    public string Description { get; set; }
}

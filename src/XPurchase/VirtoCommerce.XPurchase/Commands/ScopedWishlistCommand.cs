namespace VirtoCommerce.XPurchase.Commands;

public abstract class ScopedWishlistCommand : WishlistCommand
{
    public string Scope { get; set; }
}

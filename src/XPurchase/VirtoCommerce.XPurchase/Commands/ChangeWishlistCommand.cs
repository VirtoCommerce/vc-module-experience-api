namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeWishlistCommand : ScopedWishlistCommand
    {
        public string ListName { get; set; }

        public string Description { get; set; }
    }
}

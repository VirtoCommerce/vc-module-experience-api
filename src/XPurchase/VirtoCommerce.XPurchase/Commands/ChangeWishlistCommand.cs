namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeWishlistCommand : WishlistCommand
    {
        public string ListName { get; set; }

        public string Scope { get; set; }

        public string Description { get; set; }
    }
}

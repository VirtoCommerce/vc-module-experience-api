namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateWishlistCommand : ScopedWishlistCommand
    {
        public string ListName { get => CartName; set => CartName = value; }

        public string Description { get; set; }
    }
}

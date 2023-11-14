namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateWishlistCommand : CartCommand
    {
        public string ListName { get => CartName; set => CartName = value; }

        public string Scope { get; set; }

        public string Description { get; set; }
    }
}

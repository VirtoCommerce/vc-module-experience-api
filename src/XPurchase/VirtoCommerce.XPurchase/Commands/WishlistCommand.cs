namespace VirtoCommerce.XPurchase.Commands
{
    public abstract class WishlistCommand : CartCommand
    {
        public string ListId { get; set; }
    }
}

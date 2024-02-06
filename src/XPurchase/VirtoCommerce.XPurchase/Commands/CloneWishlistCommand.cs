namespace VirtoCommerce.XPurchase.Commands;

public class CloneWishlistCommand : CartCommand
{
    public string ListName { get => CartName; set => CartName = value; }
    public string ListId { get => CartId; set => CartId = value; }
    public string Scope { get; set; }
    public string Description { get; set; }
}

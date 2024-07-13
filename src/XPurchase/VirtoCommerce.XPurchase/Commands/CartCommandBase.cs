namespace VirtoCommerce.XPurchase.Commands;

public abstract class CartCommandBase : ICartRequest
{
    public string CartId { get; set; }
    public string StoreId { get; set; }
    public string CartType { get; set; }
    public string CartName { get; set; } = "default";
    public string UserId { get; set; }
    public string OrganizationId { get; set; }
    public string CurrencyCode { get; set; }
    public string CultureName { get; set; }

    public virtual void CopyFrom(ICartRequest source)
    {
        CartId = source.CartId;
        StoreId = source.StoreId;
        CartType = source.CartType;
        CartName = source.CartName;
        UserId = source.UserId;
        OrganizationId = source.OrganizationId;
        CurrencyCode = source.CurrencyCode;
        CultureName = source.CultureName;
    }
}

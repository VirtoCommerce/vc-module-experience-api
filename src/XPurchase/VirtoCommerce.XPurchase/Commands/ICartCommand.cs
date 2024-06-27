namespace VirtoCommerce.XPurchase.Commands;

public interface ICartCommand
{
    string StoreId { get; set; }
    string CartType { get; set; }
    string CartName { get; set; }
    string UserId { get; set; }
    string OrganizationId { get; set; }
    string CurrencyCode { get; set; }
    string CultureName { get; set; }
}

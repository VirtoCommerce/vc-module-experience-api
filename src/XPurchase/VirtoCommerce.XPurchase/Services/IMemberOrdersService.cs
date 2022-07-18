namespace VirtoCommerce.XPurchase.Services
{
    public interface IMemberOrdersService
    {
        bool IsFirstBuyer(string customerId);
    }
}

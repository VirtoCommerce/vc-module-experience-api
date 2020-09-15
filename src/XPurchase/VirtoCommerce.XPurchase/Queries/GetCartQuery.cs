namespace VirtoCommerce.XPurchase.Queries
{
    public class GetCartQuery : CartQueryBase<CartAggregate>
    {
        public GetCartQuery()
        {
        }
        public GetCartQuery(string storeId, string type, string cartName, string userId, string currencyCode, string cultureName)
        {
            StoreId = storeId;
            CartType = type;
            CartName = cartName;
            UserId = userId;
            CurrencyCode = currencyCode;
            CultureName = cultureName;
        }        

    }
}

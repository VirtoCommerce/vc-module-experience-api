using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public class GetCartQuery : IQuery<CartAggregate>
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
        
        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string CartName { get; set; }
        public string UserId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }

    }
}

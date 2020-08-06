using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchCartQuery: IQuery<SearchCartResponse>
    {
        public SearchCartQuery()
        {
        }

        public SearchCartQuery(string storeId, string type, string userId, string currencyCode, string cultureName, string sort, int skip, int take)
        {
            StoreId = storeId;
            CartType = type;
            UserId = userId;
            CurrencyCode = currencyCode;
            CultureName = cultureName;
            Sort = sort;
            Skip = skip;
            Take = take;
        }

        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string UserId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}

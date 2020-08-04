using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Queries
{
    public class GetWishListQuery: IQuery<IList<WishList>>
    {
        public GetWishListQuery()
        {
        }

        public GetWishListQuery(string storeId, string type, string userId, string currencyCode, string cultureName)
        {
            StoreId = storeId;
            CartType = type;
            UserId = userId;
            CurrencyCode = currencyCode;
            CultureName = cultureName;
        }

        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string UserId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }
    }
}

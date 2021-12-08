using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateWishlistCommand : ICommand<CartAggregate>
    {
        public string ListName { get; set; }
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }

        public CreateWishlistCommand(string listName, string storeId, string userId, string currencyCode, string cultureName)
        {
            ListName = listName;
            StoreId = storeId;
            UserId = userId;
            CurrencyCode = currencyCode;
            CultureName = cultureName;
        }
    }
}

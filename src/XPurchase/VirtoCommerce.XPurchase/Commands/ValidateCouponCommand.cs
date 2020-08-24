using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ValidateCouponCommand : ICommand<bool>
    {
        public ValidateCouponCommand()
        {
        }

        public ValidateCouponCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string coupon)
        {
            StoreId = storeId;
            CartType = cartType;
            CartName = cartName;
            UserId = userId;
            CurrencyCode = currencyCode;
            CultureName = cultureName;
            Coupon = coupon;
        }

        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string CartName { get; set; }
        public string UserId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }
        public string Coupon { get; set; }
    }
}

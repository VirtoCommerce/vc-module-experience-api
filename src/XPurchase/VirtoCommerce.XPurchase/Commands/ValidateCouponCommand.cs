using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ValidateCouponCommand : IRequest<bool>
    {
        public ValidateCouponCommand()
        {
        }

        public ValidateCouponCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string coupon)
        {
            StoreId = storeId;
            CartType = cartType;
            CartName = cartName;
            UserId = userId;
            Currency = currency;
            Language = lang;
            Coupon = coupon;
        }

        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string CartName { get; set; }
        public string UserId { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public string Coupon { get; set; }
    }
}

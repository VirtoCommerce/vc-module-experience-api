namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCouponCommand : CartCommand
    {
        public AddCouponCommand()
        {
        }

        public AddCouponCommand(string storeId, string cartType, string cartName, string userId, string currency, string lang, string couponCode)
            : base(storeId, cartType, cartName, userId, currency, lang)
        {
            CouponCode = couponCode;
        }

        public string CouponCode { get; set; }
    }
}

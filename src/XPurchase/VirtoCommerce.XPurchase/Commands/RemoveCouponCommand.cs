namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCouponCommand : CartCommand
    {
        public RemoveCouponCommand()
        {
        }

        public RemoveCouponCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string couponCode)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            CouponCode = couponCode;
        }

        public string CouponCode { get; set; }
    }
}

using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;

namespace VirtoCommerce.XPurchase.Core.Commands
{
    public class AddCouponCommand : CartCommand
    {
        public AddCouponCommand()
        {
        }

        public AddCouponCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string couponCode)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            CouponCode = couponCode;
        }

        public string CouponCode { get; set; }
    }
}

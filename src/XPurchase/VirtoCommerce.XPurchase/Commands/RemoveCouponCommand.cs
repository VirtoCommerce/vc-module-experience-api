using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCouponCommand : CartCommand
    {
        public RemoveCouponCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public RemoveCouponCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string couponCode)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            CouponCode = couponCode;
        }

        public string CouponCode { get; set; }
    }
}

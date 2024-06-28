using System;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ValidateCouponCommand : CartCommandBase, ICommand<bool>
    {
        public ValidateCouponCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
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

        public string Coupon { get; set; }
    }
}

using System;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ValidateCouponCommand : ICartCommand, IRequest<bool>
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

        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string CartName { get; set; }
        public string UserId { get; set; }
        public string OrganizationId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }

        public string Coupon { get; set; }
    }
}

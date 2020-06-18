using System.Collections.Generic;
using PlatformSecurity = VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.XPurchase.Models.Security
{
    public static class SecurityConstants
    {
        public const string AnonymousUsername = "Anonymous";

        public static readonly PlatformSecurity.Role XPurchaseCartUser = new PlatformSecurity.Role
        {
            Id = "x-purchase-cart-user",
            Name = "X Purchase cart user",
            Description = "This role allow to work with carts and create orders",
        };

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const string OperatorUserNameClaimType = "operatorname";
            public const string OperatorUserIdClaimType = "operatornameidentifier";
            public const string CurrencyClaimType = "currency";
        }

        public static class Permissions
        {
            public const string CanReadCart = "cart:read";
            public const string CanCreateCart = "cart:create";
            public const string CanUpdateCart = "cart:update";
            public const string CanDeleteCart = "cart:delete";
            public const string CanCreateOrder = "order:create";
            public const string CanReadOrder = "order:read";
            public const string CanReadOrderPrices = "order:read_prices";
            public const string CanUpdateOrder = "order:update";

            public static readonly IEnumerable<string> AllPermissions = new[]
            {
                CanCreateCart,
                CanReadCart,
                CanUpdateCart,
                CanDeleteCart,
                CanCreateOrder,
                CanReadOrder,
                CanReadOrderPrices,
                CanUpdateOrder,
            };
        }
    }
}

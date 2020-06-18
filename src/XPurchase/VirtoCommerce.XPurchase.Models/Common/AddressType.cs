using System;

namespace VirtoCommerce.XPurchase.Models.Common
{
    [Flags]
    public enum AddressType
    {
        Billing = 1,
        Shipping = 2,
        BillingAndShipping = Billing | Shipping,
    }
}

using System;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    [Flags]
    public enum AddressType
    {
        Billing = 1,
        Shipping = 2,
        BillingAndShipping = Billing | Shipping,
    }
}

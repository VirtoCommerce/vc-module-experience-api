using System;

namespace VirtoCommerce.XPurchase
{
    [Flags]
    public enum CartAggregateResponseGroup
    {
        None = 0,
        WithPayments = 1,
        WithLineItems = 1 << 1,
        WithShipments = 1 << 2,
        WithDynamicProperties = 1 << 3,
        Validate = 1 << 4,
        Full = WithPayments | WithLineItems | WithShipments | WithDynamicProperties | Validate
    }
}

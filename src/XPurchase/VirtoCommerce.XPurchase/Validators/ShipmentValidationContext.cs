using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.XPurchase.Validators
{
    public class ShipmentValidationContext
    {
        public IEnumerable<ShippingRate> AvailShippingRates { get; set; }
        public Shipment Shipment { get; set; }
    }
}

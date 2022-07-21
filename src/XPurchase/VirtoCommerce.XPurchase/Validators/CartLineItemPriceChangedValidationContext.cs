using System.Collections.Generic;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartLineItemPriceChangedValidationContext
    {
        public LineItem LineItem { get; set; }
        public IDictionary<string, CartProduct> CartProducts { get; set; } = new Dictionary<string, CartProduct>();
    }
}

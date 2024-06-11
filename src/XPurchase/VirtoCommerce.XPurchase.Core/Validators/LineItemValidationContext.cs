using System.Collections.Generic;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Validators
{
    public class LineItemValidationContext
    {
        public LineItem LineItem { get; set; }
        public IEnumerable<CartProduct> AllCartProducts { get; set; }
    }
}

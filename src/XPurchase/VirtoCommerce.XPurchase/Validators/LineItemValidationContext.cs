using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Validators
{
    public class LineItemValidationContext
    {
        public LineItem LineItem { get; set; }
        public IEnumerable<CartProduct> AllCartProducts { get; set; }
    }
}

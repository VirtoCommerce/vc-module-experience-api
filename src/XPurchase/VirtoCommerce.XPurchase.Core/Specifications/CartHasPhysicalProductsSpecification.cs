using System;
using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Core.Specifications
{
    public class CartHasPhysicalProductsSpecification
    {
        public virtual bool IsSatisfiedBy(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Items.IsNullOrEmpty())
            {
                return false;
            }

            return shoppingCart.Items.Any(i => string.IsNullOrEmpty(i.ProductType)
                     || i.ProductType.Equals("Physical", StringComparison.OrdinalIgnoreCase));
        }
    }
}

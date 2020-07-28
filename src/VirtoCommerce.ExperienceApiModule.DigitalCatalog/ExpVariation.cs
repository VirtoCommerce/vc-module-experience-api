using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpVariation : ExpProduct
    {
        public ExpVariation(CatalogProduct product, List<ProductPrice> productPrices)
        {
            CatalogProduct = product;
            ProductPrices = productPrices;
        }

        public ExpVariation(ExpProduct expProduct)
        {
            CatalogProduct = expProduct.CatalogProduct;
            ProductPrices = expProduct.ProductPrices;
        }
    }
}

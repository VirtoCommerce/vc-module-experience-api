namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpVariation : ExpProduct
    {
        public ExpVariation(ExpProduct expProduct)
        {
            CatalogProduct = expProduct.CatalogProduct;
            ProductPrices = expProduct.ProductPrices;
        }
    }
}

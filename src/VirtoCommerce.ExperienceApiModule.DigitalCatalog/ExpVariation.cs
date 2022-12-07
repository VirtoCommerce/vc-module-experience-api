namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpVariation : ExpProduct
    {
        public ExpVariation(ExpProduct expProduct)
        {
            IndexedProduct = expProduct.IndexedProduct;
            AllPrices = expProduct.AllPrices;
            AllInventories = expProduct.AllInventories;
            Vendor = expProduct.Vendor;
        }
    }
}

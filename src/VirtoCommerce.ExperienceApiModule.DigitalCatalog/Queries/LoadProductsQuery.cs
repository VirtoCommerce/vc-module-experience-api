namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductsQuery : CatalogQueryBase<LoadProductResponse>
    {
        public string[] ObjectIds { get; set; }
        public bool EvaluatePromotions { get; set; } = true;
    }
}

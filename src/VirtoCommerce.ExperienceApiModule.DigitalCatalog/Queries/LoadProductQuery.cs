namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductQuery : CatalogQueryBase<LoadProductResponse>
    {
        public string[] Ids { get; set; }
    }
}

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadCategoryQuery : CatalogQueryBase<LoadCategoryResponse>
    {
        public string[] ObjectIds { get; set; }
    }
}

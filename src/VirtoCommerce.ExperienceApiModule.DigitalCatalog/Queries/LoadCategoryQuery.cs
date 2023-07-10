using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadCategoryQuery : CatalogQueryBase<LoadCategoryResponse>
    {
        public string[] ObjectIds { get; set; }

        public bool GetLoadChildCategories()
        {
            return IncludeFields.ContainsAny("childCategories");
        }
    }
}

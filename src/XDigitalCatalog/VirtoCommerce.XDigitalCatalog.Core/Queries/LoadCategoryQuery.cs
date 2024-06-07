using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries
{
    public class LoadCategoryQuery : CatalogQueryBase<LoadCategoryResponse>
    {
        public string[] ObjectIds { get; set; }
    }
}

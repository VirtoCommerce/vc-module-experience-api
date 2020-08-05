using VirtoCommerce.ExperienceApiModule.Core.Index;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductsQuery : CatalogQueryBase<LoadProductResponse>, IGetDocumentsByIdsQuery
    {
        public string[] ObjectIds { get; set; }
    }
}

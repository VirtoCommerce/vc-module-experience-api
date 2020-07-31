using VirtoCommerce.ExperienceApiModule.Core.Index;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductQuery : CatalogQueryBase<LoadProductResponse>, IGetSingleDocumentQuery
    {
        public string ObjectId { get; set; }
    }
}

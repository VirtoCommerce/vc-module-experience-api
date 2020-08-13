using VirtoCommerce.ExperienceApiModule.Core.Index;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryQuery : CatalogQueryBase<SearchCategoryResponse>, ISearchDocumentsQuery
    {
        public string Query { get; set; }
        public bool Fuzzy { get; set; }
        public int? FuzzyLevel { get; set; }
        public string Filter { get; set; }
        public string Facet { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string[] ObjectIds { get; set; }
    }
}

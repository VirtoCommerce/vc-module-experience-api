using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchVideoQuery : IQuery<SearchVideoQueryResponse>
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string CultureName { get; set; }
        public string OwnerId { get; set; }
        public string OwnerType { get; set; }
    }
}

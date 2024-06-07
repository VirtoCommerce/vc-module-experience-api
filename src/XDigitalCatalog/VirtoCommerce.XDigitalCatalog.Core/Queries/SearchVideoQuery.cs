using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries
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

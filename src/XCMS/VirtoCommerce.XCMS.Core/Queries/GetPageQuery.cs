using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XCMS.Core.Models;

namespace VirtoCommerce.XCMS.Core.Queries
{
    public class GetPageQuery : IQuery<GetPageResponse>
    {
        public string StoreId { get; set; }
        public string CultureName { get; set; }
        public string Keyword { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
    }
}

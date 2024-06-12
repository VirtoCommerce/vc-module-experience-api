using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XCMS.Core.Models;

namespace VirtoCommerce.XCMS.Core.Queries
{
    public class GetMenusQuery : IQuery<GetMenusResponse>
    {
        public string StoreId { get; set; }
        public string CultureName { get; set; }
        public string Keyword { get; set; }
    }
}

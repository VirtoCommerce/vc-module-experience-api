using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetPageResponse
    {
        public int TotalCount { get; set; }
        public IEnumerable<PageItem> Pages { get; set; }
    }
}

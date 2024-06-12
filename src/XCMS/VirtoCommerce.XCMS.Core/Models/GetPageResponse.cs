using System.Collections.Generic;

namespace VirtoCommerce.XCMS.Core.Models
{
    public class GetPageResponse
    {
        public int TotalCount { get; set; }
        public IEnumerable<PageItem> Pages { get; set; }
    }
}

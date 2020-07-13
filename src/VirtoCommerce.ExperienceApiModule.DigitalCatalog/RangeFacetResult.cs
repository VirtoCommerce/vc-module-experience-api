using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog
{
    public sealed class RangeFacetResult : FacetResult
    {
        public RangeFacetResult()
            : base(FacetTypes.Range)
        {
        }

        public IList<FacetRange> Ranges { get; set; } = new List<FacetRange>();
    }
}

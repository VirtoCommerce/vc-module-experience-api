using System;
using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public sealed class RangeFacetResult : FacetResult
    {
        public RangeFacetResult()
            : base(FacetTypes.Range)
        {
        }

        public IList<FacetRange> Ranges { get; set; } = new List<FacetRange>();
    }
}

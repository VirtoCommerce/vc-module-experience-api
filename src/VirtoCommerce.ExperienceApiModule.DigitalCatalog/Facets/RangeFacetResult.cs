using System;
using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public sealed class RangeFacetResult_Old : FacetResult_Old
    {
        public RangeFacetResult_Old()
            : base(FacetTypes_Old.Range)
        {
        }

        public IList<FacetRange_Old> Ranges { get; set; } = new List<FacetRange_Old>();
    }
}

using System;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public sealed class FilterFacetResult_Old : FacetResult_Old
    {
        public FilterFacetResult_Old()
            : base(FacetTypes_Old.Filter)
        {
        }

        public int Count { get; set; }
    }
}

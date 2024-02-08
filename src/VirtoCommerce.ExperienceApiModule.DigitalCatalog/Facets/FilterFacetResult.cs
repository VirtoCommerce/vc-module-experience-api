using System;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public sealed class FilterFacetResult : FacetResult
    {
        public FilterFacetResult()
            : base(FacetTypes.Filter)
        {
        }

        public int Count { get; set; }
    }
}

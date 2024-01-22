using System;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public sealed class FacetFilter_Old
    {
        public string Term { get; set; }
        public int Count { get; set; }
        public bool IsSelected { get; set; }
    }
}

using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    public sealed class FacetTerm : ValueObject
    {
        public string Term { get; set; }
        public string Label { get; set; }
        public long Count { get; set; }
        public bool IsSelected { get; set; }
    }
}

using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog
{
    public sealed class FacetTerm : ValueObject
    {
        public string Term { get; set; }
        public long Count { get; set; }
        public bool IsSelected { get; set; }
    }
}

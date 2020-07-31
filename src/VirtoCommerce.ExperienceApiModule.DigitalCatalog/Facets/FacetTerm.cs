using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    public sealed class FacetTerm : ValueObject
    {
        public string Term { get; set; }
        //TODO: Need to load for requested language from index. Need to change indexation logic to be able index property values along with localized names
        public string Label { get; set; }
        public long Count { get; set; }
        public bool IsSelected { get; set; }
    }
}

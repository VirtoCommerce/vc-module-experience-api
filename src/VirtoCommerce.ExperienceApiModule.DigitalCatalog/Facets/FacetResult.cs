using System;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public abstract class FacetResult
    {
        protected FacetResult(FacetTypes facetType)
        {
            FacetType = facetType;
        }

        public string Name { get; set; }
        public string Label { get; set; }
        public string DisplayStyle { get; set; }
        public FacetTypes FacetType { get; private set; }
    }
}

using System;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public abstract class FacetResult_Old
    {
        protected FacetResult_Old(FacetTypes_Old facetType)
        {
            FacetType = facetType;
        }

        public string Name { get; set; }
        public string Label { get; set; }
        public string DisplayStyle { get; set; }
        public FacetTypes_Old FacetType { get; private set; }
    }
}

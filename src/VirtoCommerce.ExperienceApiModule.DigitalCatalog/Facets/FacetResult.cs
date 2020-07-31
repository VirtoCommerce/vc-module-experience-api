namespace VirtoCommerce.XDigitalCatalog.Facets
{
    public abstract class FacetResult
    {
        protected FacetResult(FacetTypes facetType)
        {
            FacetType = facetType;
        }

        public string Name { get; set; }
        //TODO: Need to load for requested language from index. Need to change indexation logic to be able index property display names along with system properties names
        public string Label { get; set; }
        public string DisplayStyle { get; set; }
        public FacetTypes FacetType { get; private set; }
    }
}

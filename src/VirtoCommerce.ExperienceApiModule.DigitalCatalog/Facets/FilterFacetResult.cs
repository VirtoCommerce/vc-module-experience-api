namespace VirtoCommerce.XDigitalCatalog.Facets
{
    public sealed class FilterFacetResult : FacetResult
    {
        public FilterFacetResult()
            : base(FacetTypes.Filter)
        {
        }

        public int Count { get; set; }
    }
}

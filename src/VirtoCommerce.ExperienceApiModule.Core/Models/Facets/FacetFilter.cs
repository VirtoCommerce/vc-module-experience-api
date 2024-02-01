namespace VirtoCommerce.ExperienceApiModule.Core.Models.Facets
{
    public sealed class FacetFilter
    {
        public string Term { get; set; }
        public int Count { get; set; }
        public bool IsSelected { get; set; }
    }
}

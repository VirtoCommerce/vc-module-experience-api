namespace VirtoCommerce.ExperienceApiModule.Core.Index
{
    public interface IHaveSearchParams
    {
        string Query { get; set; }
        bool Fuzzy { get; set; }
        int? FuzzyLevel { get; set; }
        string Filter { get; set; }
        string Facet { get; set; }
        string Sort { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
    }
}

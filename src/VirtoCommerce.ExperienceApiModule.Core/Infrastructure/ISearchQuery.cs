namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

public interface ISearchQuery
{
    public string Keyword { get; set; }
    public string Sort { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}

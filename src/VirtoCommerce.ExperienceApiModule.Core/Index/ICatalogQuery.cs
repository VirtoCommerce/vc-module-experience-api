namespace VirtoCommerce.ExperienceApiModule.Core.Index
{
    public interface ICatalogQuery : IHasIncludeFields
    {
        string StoreId { get; set; }
        string UserId { get; set; }
        string CultureName { get; set; }
        string CurrencyCode { get; set; }
    }
}

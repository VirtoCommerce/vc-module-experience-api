using VirtoCommerce.ExperienceApiModule.Core.Index;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public interface ICatalogQuery : IHasIncludeFields
    {
        string StoreId { get; set; }
        string UserId { get; set; }
        string OrganizationId { get; set; }
        string CultureName { get; set; }
        string CurrencyCode { get; set; }
    }
}

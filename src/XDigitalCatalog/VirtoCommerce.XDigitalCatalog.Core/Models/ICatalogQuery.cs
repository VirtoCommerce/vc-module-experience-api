using VirtoCommerce.ExperienceApiModule.Core.Index;

namespace VirtoCommerce.XDigitalCatalog.Core.Models
{
    public interface ICatalogQuery : IHasIncludeFields
    {
        string StoreId { get; set; }
        string UserId { get; set; }
        string CultureName { get; set; }
        string CurrencyCode { get; set; }
    }
}

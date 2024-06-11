using VirtoCommerce.ExperienceApiModule.Core.Index;

namespace VirtoCommerce.XPurchase.Core.Queries
{
    public interface ICartQuery : IHasIncludeFields
    {
        string StoreId { get; set; }
        string CartType { get; set; }
        string CartName { get; set; }
        string UserId { get; set; }
        string CurrencyCode { get; set; }
        string CultureName { get; set; }
    }
}

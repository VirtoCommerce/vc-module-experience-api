using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Schemas
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

    public class CartQueryBase<TResponse> : ICartQuery, IQuery<TResponse>
    {
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public string StoreId { get; set; }
        public string CartType { get; set; }
        public string CartName { get; set; }
        public string UserId { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureName { get; set; }
    }
}

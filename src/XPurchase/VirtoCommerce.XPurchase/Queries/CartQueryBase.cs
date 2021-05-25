using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
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

        public virtual string GetResponseGroup()
        {
            var result = CartAggregateResponseGroup.None;
            if (IncludeFields.Any(x => x.Contains("shipments")))
            {
                result |= CartAggregateResponseGroup.WithShipments;
            }
            if (IncludeFields.Any(x => x.Contains("payments")))
            {
                result |= CartAggregateResponseGroup.WithPayments;
            }
            if (IncludeFields.Any(x => x.Contains("items")))
            {
                result |= CartAggregateResponseGroup.WithLineItems;
            }
            if (IncludeFields.Any(x => x.Contains("dynamicProperties")))
            {
                result |= CartAggregateResponseGroup.WithDynamicProperties;
            }
            if (IncludeFields.Any(x => x.Contains("validationErrors")))
            {
                //TODO: Need take into account in the repository
                result |= CartAggregateResponseGroup.Validate;
            }

            return result.ToString();
        }

    }
}

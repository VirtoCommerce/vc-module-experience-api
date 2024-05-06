using System.Collections.Generic;

namespace VirtoCommerce.XPurchase
{
    /// <summary>
    /// Result of bulk add operation
    /// Contains error for product skus in request that were not added to the cart 
    /// </summary>
    public class BulkCartResult
    {
        public CartAggregate Cart { get; set; }

        public IList<CartValidationError> Errors { get; set; } = new List<CartValidationError>();
    }
}

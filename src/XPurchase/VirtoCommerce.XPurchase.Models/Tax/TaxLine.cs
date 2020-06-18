using System.Collections.Generic;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Tax
{
    public partial class TaxLine : CloneableEntity
    {
        public TaxLine(Currency currency)
        {
            Amount = new Money(currency);
            Price = new Money(currency);
        }

        /// <summary>
        /// represent  original object code (lineItem, shipment etc).
        /// </summary>
        public string Code { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Tax line total amount.
        /// </summary>
        public Money Amount { get; set; }

        public int Quantity { get; set; }

        /// <summary>
        /// Tax line one item price.
        /// </summary>
        public Money Price { get; set; }

        public string TaxType { get; set; }

        public string TypeName { get; set; }

        public IList<TaxDetail> TaxDetails { get; set; } = new List<TaxDetail>();
    }
}

using System.Collections.Generic;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class TaxCategory
    {
        public IEnumerable<TaxRate> Rates { get; set; }
    }
}

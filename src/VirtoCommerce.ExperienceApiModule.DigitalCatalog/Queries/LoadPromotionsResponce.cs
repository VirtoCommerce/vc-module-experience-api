using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadPromotionsResponce
    {
        public IDictionary<string, Promotion> Promotions { get; set; }
    }
}

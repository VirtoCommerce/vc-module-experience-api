using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductPricesQuery : IQuery<LoadProductPricesResponce>
    {
        public string ProductId { get; set; }
        public Language Language { get; set; }
    }
}

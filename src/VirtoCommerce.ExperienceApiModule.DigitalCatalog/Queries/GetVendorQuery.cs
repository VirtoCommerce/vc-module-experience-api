using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class GetVendorQuery: IQuery<Member>
    {
        public string Id { get; set; }
    }
}

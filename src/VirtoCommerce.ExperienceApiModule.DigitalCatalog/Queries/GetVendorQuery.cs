using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class GetVendorQuery: IQuery<ExpVendor>
    {
        public string Id { get; set; }
    }
}

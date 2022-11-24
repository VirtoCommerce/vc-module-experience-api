using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class GetVendorQuery: IQuery<ExpVendorType>
    {
        public string Id { get; set; }
    }
}

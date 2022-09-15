using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas;

public class VendorType: ExtendableGraphType<ExpVendorType>
{
    public VendorType()
    {
        Name = "Vendor";

        Field(x => x.Id, nullable: false).Description("Vendor ID");
        Field(x => x.Name, nullable: false).Description("Vendor Name");
    }
}

using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas;

public class ProductVendorType: ExtendableGraphType<ExpProductVendor>
{
    public ProductVendorType()
    {
        Name = "ProductVendor";

        Field(x => x.Id, nullable: false).Description("Vendor ID");
        Field(x => x.Name, nullable: false).Description("Vendor name");
        Field(
            GraphTypeExtenstionHelper.GetActualType<RatingType>(),
            "rating",
            "Vendor rating",
            resolve: context =>
            {
                var result = context.Source.Rating;
                return result;
            });
    }
}

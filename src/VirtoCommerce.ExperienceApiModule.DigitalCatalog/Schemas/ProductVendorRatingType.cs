using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas;

public class ProductVendorRatingType: ExtendableGraphType<ExpProductVendorRating>
{
    public ProductVendorRatingType()
    {
        Name = "Rating";

        Field(x => x.Value, nullable: false).Description("Average rating");
        Field(x => x.ReviewCount, nullable: false).Description("Total count of customer reviews");
    }
}

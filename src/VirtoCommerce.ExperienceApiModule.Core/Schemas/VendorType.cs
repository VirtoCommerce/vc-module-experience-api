using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class VendorType : ExtendableGraphType<ExpVendor>
    {
        public VendorType()
        {
            Name = "CommonVendor";

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
}

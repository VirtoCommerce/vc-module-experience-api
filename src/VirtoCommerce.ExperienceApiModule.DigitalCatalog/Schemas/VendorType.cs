using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas;

public class VendorType: ExtendableGraphType<ExpVendorType>
{
    public VendorType(IMediator mediator)
    {
        Name = "Vendor";

        Field(x => x.Id, nullable: false).Description("Vendor ID");
        Field(x => x.Name, nullable: false).Description("Vendor name");

        FieldAsync(
            GraphTypeExtenstionHelper.GetActualType<RatingType>(),
            "rating",
            "Vendor rating",
            resolve: async context =>
            {
                var storeId = context.GetArgumentOrValue<string>("storeId");
                var query = AbstractTypeFactory<GetRatingQuery>.TryCreateInstance();
                query.StoreId = storeId;
                query.EntityId = context.Source.Id;
                query.EntityType = context.Source.Type;
                var result = await mediator.Send(query);
                return result;
            });
    }
}

using GraphQL.Types;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class CoreShippingMethodType : ObjectGraphType<ShippingMethod>
    {
        public CoreShippingMethodType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.Code, nullable: false);
            Field(x => x.LogoUrl, nullable: true);
        }
    }
}

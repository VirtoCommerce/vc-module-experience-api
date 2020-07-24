using GraphQL.Types;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputOrderShippingMethodType : InputObjectGraphType<ShippingMethod>
    {
        public InputOrderShippingMethodType()
        {
            Field(x => x.Id);
            Field(x => x.Code);
            Field(x => x.LogoUrl);
            Field(x => x.IsActive);
            Field(x => x.Priority);
            Field(x => x.TaxType, true);
            Field(x => x.StoreId);
            Field(x => x.TypeName);
        }
    }
}

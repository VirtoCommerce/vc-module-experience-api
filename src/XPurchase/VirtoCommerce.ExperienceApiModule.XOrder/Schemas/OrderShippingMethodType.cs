using GraphQL.Types;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShippingMethodType : ObjectGraphType<ShippingMethod>
    {
        public OrderShippingMethodType()
        {
            Field(x => x.Id);
            Field(x => x.Code);
            Field(x => x.Name);
            Field(x => x.LogoUrl, nullable: true);
            Field(x => x.IsActive);
            Field(x => x.Priority);
            Field(x => x.TaxType, nullable: true);
            Field(x => x.StoreId);
            Field(x => x.TypeName);
        }
    }
}

using GraphQL.Types;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class OrderShippingMethodType : ObjectGraphType<ShippingMethod>
    {
        public OrderShippingMethodType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.Code, nullable: false);
            Field(x => x.Name, nullable: true);
            Field(x => x.Description, nullable: true);
            Field(x => x.LogoUrl, nullable: true);
            Field(x => x.IsActive, nullable: false);
            Field(x => x.Priority, nullable: false);
            Field(x => x.TaxType, nullable: true);
            Field(x => x.StoreId, nullable: true);
            Field(x => x.TypeName, nullable: true);
        }
    }
}

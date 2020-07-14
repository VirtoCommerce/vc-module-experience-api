using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShipmentPackageType : ObjectGraphType<ShipmentPackage>
    {
        public OrderShipmentPackageType()
        {
            Field(x => x.Id);
            Field(x => x.BarCode);
            Field(x => x.PackageType);
            Field(x => x.WeightUnit);
            Field(x => x.Weight);
            Field(x => x.MeasureUnit);
            Field(x => x.Height);
            Field(x => x.Length);
            Field(x => x.Width);
            Field<NonNullGraphType<ListGraphType<OrderShipmentItemType>>>(nameof(ShipmentPackage.Items), resolve: x => x.Source.Items);
        }
    }
}

using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShipmentPackageType : ObjectGraphType<ShipmentPackage>
    {
        public OrderShipmentPackageType()
        {
            Field(x => x.Id);
            Field(x => x.BarCode, true);
            Field(x => x.PackageType, true);
            Field(x => x.WeightUnit);
            Field(x => x.Weight, true);
            Field(x => x.MeasureUnit);
            Field(x => x.Height, true);
            Field(x => x.Length, true);
            Field(x => x.Width, true);
            Field<NonNullGraphType<ListGraphType<OrderShipmentItemType>>>(nameof(ShipmentPackage.Items), resolve: x => x.Source.Items);
        }
    }
}

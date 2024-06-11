using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class OrderShipmentPackageType : ObjectGraphType<ShipmentPackage>
    {
        public OrderShipmentPackageType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.BarCode, nullable: true);
            Field(x => x.PackageType, nullable: true);
            Field(x => x.WeightUnit, nullable: true);
            Field(x => x.Weight, nullable: true);
            Field(x => x.MeasureUnit, nullable: true);
            Field(x => x.Height, nullable: true);
            Field(x => x.Length, nullable: true);
            Field(x => x.Width, nullable: true);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<OrderShipmentItemType>>>>(nameof(ShipmentPackage.Items), resolve: x => x.Source.Items);
        }
    }
}

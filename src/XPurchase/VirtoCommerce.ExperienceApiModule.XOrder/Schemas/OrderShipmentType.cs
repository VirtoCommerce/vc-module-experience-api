using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderShipmentType : ObjectGraphType<Shipment>
    {
        public OrderShipmentType()
        {
            Field(x => x.Id);
            Field(x => x.OrganizationId);
            Field(x => x.OrganizationName);
            Field(x => x.FulfillmentCenterId);
            Field(x => x.FulfillmentCenterName);
            Field(x => x.EmployeeId);
            Field(x => x.EmployeeName);
            Field(x => x.ShipmentMethodCode);
            Field(x => x.ShipmentMethodOption);
            Field<OrderShippingMethodType>(nameof(Shipment.ShippingMethod), resolve: x => x.Source.ShippingMethod);
            Field(x => x.CustomerOrderId);
            Field(x => x.WeightUnit);
            Field(x => x.Weight, true);
            Field(x => x.MeasureUnit);
            Field(x => x.Height, true);
            Field(x => x.Length, true);
            Field(x => x.Width, true);
            Field<OrderAddressType>(nameof(Shipment.DeliveryAddress), resolve: x => x.Source.DeliveryAddress);
            Field(x => x.Price);
            Field(x => x.PriceWithTax);
            Field(x => x.Total);
            Field(x => x.TotalWithTax);
            Field(x => x.DiscountAmount);
            Field(x => x.DiscountAmountWithTax);
            Field(x => x.Fee);
            Field(x => x.FeeWithTax);
            Field(x => x.ObjectType);
            Field(x => x.TaxType);
            Field(x => x.TaxTotal);
            Field(x => x.TaxPercentRate);

            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(Shipment.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<OrderShipmentItemType>>>(nameof(Shipment.Items), resolve: x => x.Source.Items);
            Field<NonNullGraphType<ListGraphType<OrderShipmentPackageType>>>(nameof(Shipment.Packages), resolve: x => x.Source.Packages);
            Field<NonNullGraphType<ListGraphType<PaymentInType>>>(nameof(Shipment.InPayments), resolve: x => x.Source.InPayments);
            Field<NonNullGraphType<ListGraphType<OrderDiscountType>>>(nameof(Shipment.Discounts), resolve: x => x.Source.Discounts);
        }
    }
}

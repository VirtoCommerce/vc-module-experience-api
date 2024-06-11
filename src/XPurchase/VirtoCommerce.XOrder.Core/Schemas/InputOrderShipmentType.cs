using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputOrderShipmentType : InputObjectGraphType<Shipment>
    {
        public InputOrderShipmentType()
        {
            Field(x => x.Id);
            Field(x => x.ObjectType, true);
            Field(x => x.CancelReason, true);
            Field(x => x.CancelledDate, true);
            Field(x => x.IsCancelled);
            Field(x => x.OuterId, true);
            Field(x => x.Currency);
            Field(x => x.Comment, true);
            Field(x => x.Status, true);
            Field(x => x.IsApproved);
            Field(x => x.Number);
            Field(x => x.ParentOperationId, true);
            Field(x => x.OperationType);

            Field(x => x.OrganizationId, true);
            Field(x => x.OrganizationName, true);
            Field(x => x.FulfillmentCenterId, true);
            Field(x => x.FulfillmentCenterName, true);
            Field(x => x.EmployeeId, true);
            Field(x => x.EmployeeName, true);
            Field(x => x.ShipmentMethodCode, true);
            Field(x => x.ShipmentMethodOption, true);
            Field<InputOrderShippingMethodType>(nameof(Shipment.ShippingMethod),
                "Shipping method");
            Field(x => x.CustomerOrderId);
            Field(x => x.WeightUnit, true);
            Field(x => x.Weight, true);
            Field(x => x.MeasureUnit, true);
            Field(x => x.Height, true);
            Field(x => x.Length, true);
            Field(x => x.Width, true);
            Field<InputOrderAddressType>(nameof(Shipment.DeliveryAddress),
                "Delivery address");

            Field(x => x.TaxType, true);
            Field(x => x.TaxPercentRate);

            Field(x => x.Price);
            Field(x => x.PriceWithTax);
            Field(x => x.Total);
            Field(x => x.TotalWithTax);
            Field(x => x.DiscountAmount);
            Field(x => x.DiscountAmountWithTax);
            Field(x => x.TaxTotal);

            Field(x => x.TrackingNumber, true);
            Field(x => x.TrackingUrl, true);
            Field(x => x.DeliveryDate, true);

            Field<NonNullGraphType<ListGraphType<InputOrderTaxDetailType>>>(nameof(Shipment.TaxDetails),
                "Tax details");
            Field<NonNullGraphType<ListGraphType<InputOrderShipmentItemType>>>(nameof(Shipment.Items),
                "Shipment items");
            Field<NonNullGraphType<ListGraphType<InputOrderShipmentPackageType>>>(nameof(Shipment.Packages));
            Field<NonNullGraphType<ListGraphType<InputOrderPaymentType>>>(nameof(Shipment.InPayments));
            Field<NonNullGraphType<ListGraphType<InputOrderDiscountType>>>(nameof(Shipment.Discounts),
                "Discounts");
        }
    }
}

using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class ShipmentType : ObjectGraphType<Shipment>
    {
        public ShipmentType()
        {
            Field(x => x.OrganizationId);
            Field(x => x.OrganizationName);
            Field(x => x.FulfillmentCenterId);
            Field(x => x.FulfillmentCenterName);
            Field(x => x.EmployeeId);
            Field(x => x.EmployeeName);
            Field(x => x.ShipmentMethodCode);
            Field(x => x.ShipmentMethodOption);
            //public ShippingMethod ShippingMethod);
            Field(x => x.CustomerOrderId);
            //public CustomerOrder CustomerOrder);
            //public ICollection<ShipmentItem> Items);
            //public ICollection<ShipmentPackage> Packages);
            //public ICollection<PaymentIn> InPayments);
            Field(x => x.WeightUnit);
            Field(x => x.Weight);
            Field(x => x.MeasureUnit);
            Field(x => x.Height);
            Field(x => x.Length);
            Field(x => x.Width);
            //public ICollection<Discount> Discounts);
            //public Address DeliveryAddress);
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
        }
    }
}

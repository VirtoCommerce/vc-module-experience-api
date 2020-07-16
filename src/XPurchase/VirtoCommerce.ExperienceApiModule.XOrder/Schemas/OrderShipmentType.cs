using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
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
            Field<OrderShippingMethodType>(nameof(Shipment.ShippingMethod).ToCamelCase(), resolve: x => x.Source.ShippingMethod);
            Field(x => x.CustomerOrderId);
            Field(x => x.WeightUnit);
            Field(x => x.Weight, true);
            Field(x => x.MeasureUnit);
            Field(x => x.Height, true);
            Field(x => x.Length, true);
            Field(x => x.Width, true);
            Field<OrderAddressType>(nameof(Shipment.DeliveryAddress).ToCamelCase(), resolve: x => x.Source.DeliveryAddress);
            
            Field(x => x.Fee);
            Field(x => x.FeeWithTax);
            Field(x => x.ObjectType);
            Field(x => x.TaxType);
            Field(x => x.TaxPercentRate);

            Field<OrderMoneyType>(nameof(Shipment.Price).ToCamelCase(), resolve: context =>  new Money(context.Source.Price, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(Shipment.PriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.PriceWithTax, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(Shipment.Total).ToCamelCase(), resolve: context => new Money(context.Source.Total, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(Shipment.TotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.TotalWithTax, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(Shipment.DiscountAmount).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmount,context. GetOrder().Currency));
            Field<OrderMoneyType>(nameof(Shipment.DiscountAmountWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(Shipment.TaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.TaxTotal,context.GetOrder().Currency));
            Field<OrderCurrencyType>(nameof(Shipment.Currency).ToCamelCase(), resolve: context => context.GetOrder().Currency);

            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(Shipment.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<OrderShipmentItemType>>>(nameof(Shipment.Items), resolve: x => x.Source.Items);
            Field<NonNullGraphType<ListGraphType<OrderShipmentPackageType>>>(nameof(Shipment.Packages), resolve: x => x.Source.Packages);
            Field<NonNullGraphType<ListGraphType<PaymentInType>>>(nameof(Shipment.InPayments), resolve: x => x.Source.InPayments);
            Field<NonNullGraphType<ListGraphType<OrderDiscountType>>>(nameof(Shipment.Discounts), resolve: x => x.Source.Discounts);
        }
    }
}

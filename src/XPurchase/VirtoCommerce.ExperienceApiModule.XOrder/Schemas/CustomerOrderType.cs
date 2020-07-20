using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class CustomerOrderType : ObjectGraphType<CustomerOrderAggregate> 
    {
        public CustomerOrderType()
        {
            Field(x => x.Order.Id);
            Field(x => x.Order.OperationType);
            Field(x => x.Order.ParentOperationId, true);
            Field(x => x.Order.Number);
            Field(x => x.Order.IsApproved);
            Field(x => x.Order.Status, true);
            Field(x => x.Order.Comment, true);
            Field(x => x.Order.Sum);
            Field(x => x.Order.OuterId, true);
            Field(x => x.Order.IsCancelled);
            Field(x => x.Order.CancelledDate, true);
            Field(x => x.Order.CancelReason, true);
            Field(x => x.Order.ObjectType);
            Field(x => x.Order.CustomerId);
            Field(x => x.Order.CustomerName, true);
            Field(x => x.Order.ChannelId, true);
            Field(x => x.Order.StoreId);
            Field(x => x.Order.StoreName, true);
            Field(x => x.Order.OrganizationId, true);
            Field(x => x.Order.OrganizationName, true);
            Field(x => x.Order.EmployeeId, true);
            Field(x => x.Order.EmployeeName, true);
            Field(x => x.Order.ShoppingCartId, true);
            Field(x => x.Order.IsPrototype);
            Field(x => x.Order.SubscriptionNumber, true);
            Field(x => x.Order.SubscriptionId, true);
            Field(x => x.Order.Fee);
            Field(x => x.Order.FeeWithTax);
            Field(x => x.Order.FeeTotal);
            Field(x => x.Order.FeeTotalWithTax);
            Field(x => x.Order.TaxType, true);
            
            Field(x => x.Order.TaxPercentRate);
            Field(x => x.Order.LanguageCode, true);
            Field(x => x.Order.CreatedDate);
            Field(x => x.Order.CreatedBy, true);
            Field(x => x.Order.ModifiedDate, true);
            Field(x => x.Order.ModifiedBy, true);

            Field<OrderCurrencyType>(nameof(CustomerOrder.Currency).ToCamelCase(), resolve: context => context.Source.Currency);
            Field<OrderMoneyType>(nameof(CustomerOrder.Total).ToCamelCase(), resolve: context => new Money(context.Source.Order.Total, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.TaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.TaxTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.DiscountAmount).ToCamelCase(), resolve: context => new Money(context.Source.Order.DiscountAmount, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.SubTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.SubTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotalWithTax, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.SubTotalDiscount).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotalDiscount, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.SubTotalDiscountWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotalDiscountWithTax, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.SubTotalTaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotalTaxTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.ShippingTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.ShippingTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingTotalWithTax, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.ShippingSubTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingSubTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.ShippingSubTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingSubTotalWithTax, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.ShippingDiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingDiscountTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.ShippingDiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingDiscountTotalWithTax, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.ShippingTaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingTaxTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.PaymentTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.PaymentTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentTotalWithTax, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.PaymentSubTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentSubTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.PaymentSubTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentSubTotalWithTax, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.PaymentDiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentDiscountTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.PaymentDiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentDiscountTotalWithTax, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.PaymentTaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentTaxTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.DiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.DiscountTotal, context.Source.Currency));
            Field<OrderMoneyType>(nameof(CustomerOrder.DiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.DiscountTotalWithTax, context.Source.Currency));

            Field<NonNullGraphType<ListGraphType<OrderAddressType>>>(nameof(CustomerOrder.Addresses), resolve: x => x.Source.Order.Addresses);
            Field<NonNullGraphType<ListGraphType<OrderLineItemType>>>(nameof(CustomerOrder.Items), resolve: x => x.Source.Order.Items);
            Field<NonNullGraphType<ListGraphType<PaymentInType>>>(nameof(CustomerOrder.InPayments), resolve: x => x.Source.Order.InPayments);
            Field<NonNullGraphType<ListGraphType<OrderShipmentType>>>(nameof(CustomerOrder.Shipments), resolve: x => x.Source.Order.Shipments);
            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(CustomerOrder.TaxDetails), resolve: x => x.Source.Order.TaxDetails);
            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }
    }
}

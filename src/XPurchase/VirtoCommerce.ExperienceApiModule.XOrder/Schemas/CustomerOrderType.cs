using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class CustomerOrderType : ExtendableGraphType<CustomerOrderAggregate>
    {
        public CustomerOrderType(IDynamicPropertyResolverService dynamicPropertyResolverService)
        {
            Field(x => x.Order.Id);
            Field(x => x.Order.OperationType);
            Field(x => x.Order.ParentOperationId, true);
            Field(x => x.Order.Number);
            Field(x => x.Order.IsApproved);
            Field(x => x.Order.Status, true);
            Field(x => x.Order.Comment, true);
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
            Field(x => x.Order.PurchaseOrderNumber, true);
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

            Field<CurrencyType>(nameof(CustomerOrder.Currency).ToCamelCase(), resolve: context => context.Source.Currency);
            Field<MoneyType>(nameof(CustomerOrder.Total).ToCamelCase(), resolve: context => new Money(context.Source.Order.Total, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.TaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.TaxTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.DiscountAmount).ToCamelCase(), resolve: context => new Money(context.Source.Order.DiscountAmount, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.SubTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.SubTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotalWithTax, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.SubTotalDiscount).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotalDiscount, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.SubTotalDiscountWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotalDiscountWithTax, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.SubTotalTaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.SubTotalTaxTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.ShippingTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.ShippingTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingTotalWithTax, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.ShippingSubTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingSubTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.ShippingSubTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingSubTotalWithTax, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.ShippingDiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingDiscountTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.ShippingDiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingDiscountTotalWithTax, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.ShippingTaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.ShippingTaxTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.PaymentTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.PaymentTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentTotalWithTax, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.PaymentSubTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentSubTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.PaymentSubTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentSubTotalWithTax, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.PaymentDiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentDiscountTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.PaymentDiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentDiscountTotalWithTax, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.PaymentTaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.PaymentTaxTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.DiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.Order.DiscountTotal, context.Source.Currency));
            Field<MoneyType>(nameof(CustomerOrder.DiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.Order.DiscountTotalWithTax, context.Source.Currency));

            ExtendableField<NonNullGraphType<ListGraphType<AddressType>>>(nameof(CustomerOrder.Addresses), resolve: x => x.Source.Order.Addresses);
            ExtendableField<NonNullGraphType<ListGraphType<OrderLineItemType>>>(nameof(CustomerOrder.Items), resolve: x => x.Source.Order.Items);
            ExtendableField<NonNullGraphType<ListGraphType<PaymentInType>>>(nameof(CustomerOrder.InPayments), resolve: x => x.Source.Order.InPayments);
            ExtendableField<ListGraphType<OrderShipmentType>>(nameof(CustomerOrder.Shipments), resolve: x => x.Source.Order.Shipments);

            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(CustomerOrder.TaxDetails), resolve: x => x.Source.Order.TaxDetails);

            ExtendableField<ListGraphType<DynamicPropertyValueType>>(
                "dynamicProperties",
                "Customer order dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source.Order, context.GetArgumentOrValue<string>("cultureName")));

            ExtendableField<ListGraphType<StringGraphType>>("coupons", resolve: x => x.Source.GetCustomerOrderCoupons());
        }
    }
}

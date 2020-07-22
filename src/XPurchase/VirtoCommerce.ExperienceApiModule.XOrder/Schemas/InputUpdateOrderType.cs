using GraphQL;
using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputUpdateOrderType : InputObjectGraphType<CustomerOrder>
    {
        public InputUpdateOrderType()
        {
            Field(x => x.Id);
            Field(x => x.ObjectType, true);
            Field(x => x.CancelReason, true);
            Field(x => x.CancelledDate, true);
            Field(x => x.IsCancelled);
            Field(x => x.OuterId, true);
            Field(x => x.Sum);
            Field(x => x.Currency);
            Field(x => x.Comment, true);
            Field(x => x.Status, true);
            Field(x => x.IsApproved);
            Field(x => x.Number);
            Field(x => x.ParentOperationId, true);
            Field(x => x.OperationType, true);
            Field(x => x.CreatedDate, true);

            Field(x => x.CustomerId);
            Field(x => x.CustomerName, true);
            Field(x => x.ChannelId, true);
            Field(x => x.StoreId);
            Field(x => x.StoreName, true);
            Field(x => x.OrganizationId, true);
            Field(x => x.OrganizationName, true);
            Field(x => x.EmployeeId, true);
            Field(x => x.EmployeeName, true);
            Field(x => x.ShoppingCartId, true);
            Field(x => x.IsPrototype, true);
            Field(x => x.SubscriptionNumber, true);
            Field(x => x.SubscriptionId, true);
            Field(x => x.Fee, true);
            Field(x => x.FeeWithTax, true);
            Field(x => x.FeeTotal, true);
            Field(x => x.FeeTotalWithTax, true);
            Field(x => x.TaxType, true);
            Field(x => x.TaxPercentRate, true);
            Field(x => x.LanguageCode);
            Field(x => x.Total);
            Field(x => x.TaxTotal);
            Field(x => x.DiscountAmount);
            Field(x => x.SubTotal);
            Field(x => x.SubTotalWithTax);
            Field(x => x.SubTotalDiscount);
            Field(x => x.SubTotalDiscountWithTax);
            Field(x => x.SubTotalTaxTotal);
            Field(x => x.ShippingTotal, true);
            Field(x => x.ShippingTotalWithTax);
            Field(x => x.ShippingSubTotal, true);
            Field(x => x.ShippingSubTotalWithTax, true);
            Field(x => x.ShippingDiscountTotal, true);
            Field(x => x.ShippingDiscountTotalWithTax, true);
            Field(x => x.ShippingTaxTotal);
            Field(x => x.PaymentTotal);
            Field(x => x.PaymentTotalWithTax);
            Field(x => x.PaymentSubTotal, true);
            Field(x => x.PaymentSubTotalWithTax, true);
            Field(x => x.PaymentDiscountTotal);
            Field(x => x.PaymentDiscountTotalWithTax);
            Field(x => x.PaymentTaxTotal);
            Field(x => x.DiscountTotal);
            Field(x => x.DiscountTotalWithTax);

            Field<NonNullGraphType<ListGraphType<InputOrderAddressType>>>(nameof(CustomerOrder.Addresses));
            Field<NonNullGraphType<ListGraphType<InputOrderLineItemType>>>(nameof(CustomerOrder.Items));
            Field<NonNullGraphType<ListGraphType<InputPaymentInType>>>(nameof(CustomerOrder.InPayments));
            Field<NonNullGraphType<ListGraphType<InputOrderShipmentType>>>(nameof(CustomerOrder.Shipments));
            Field<NonNullGraphType<ListGraphType<InputOrderTaxDetailType>>>(nameof(CustomerOrder.TaxDetails));
            Field<NonNullGraphType<ListGraphType<InputOrderDiscountType>>>(nameof(CustomerOrder.Discounts));
            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }

    }
}

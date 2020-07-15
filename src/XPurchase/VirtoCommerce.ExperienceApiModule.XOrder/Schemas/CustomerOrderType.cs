using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class CustomerOrderType : ObjectGraphType<CustomerOrder> 
    {
        public CustomerOrderType()
        {
            Field(x => x.Id);
            Field(x => x.OperationType);
            Field(x => x.ParentOperationId);
            Field(x => x.Number);
            Field(x => x.IsApproved);
            Field(x => x.Status, true);
            Field(x => x.Comment, true);
            Field(x => x.Currency);
            Field(x => x.Sum);
            Field(x => x.OuterId, true);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate, true);
            Field(x => x.CancelReason, true);
            Field(x => x.ObjectType);
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
            Field(x => x.IsPrototype);
            Field(x => x.SubscriptionNumber, true);
            Field(x => x.SubscriptionId, true);
            Field(x => x.DiscountAmount);
            Field(x => x.Total);
            Field(x => x.SubTotal);
            Field(x => x.SubTotalWithTax);
            Field(x => x.SubTotalDiscount);
            Field(x => x.SubTotalDiscountWithTax);
            Field(x => x.SubTotalTaxTotal);
            Field(x => x.ShippingTotal);
            Field(x => x.ShippingTotalWithTax);
            Field(x => x.ShippingSubTotal);
            Field(x => x.ShippingSubTotalWithTax);
            Field(x => x.ShippingDiscountTotal);
            Field(x => x.ShippingDiscountTotalWithTax);
            Field(x => x.ShippingTaxTotal);
            Field(x => x.PaymentTotal);
            Field(x => x.PaymentTotalWithTax);
            Field(x => x.PaymentSubTotal);
            Field(x => x.PaymentSubTotalWithTax);
            Field(x => x.PaymentDiscountTotal);
            Field(x => x.PaymentDiscountTotalWithTax);
            Field(x => x.PaymentTaxTotal);
            Field(x => x.DiscountTotal);
            Field(x => x.DiscountTotalWithTax);
            Field(x => x.Fee);
            Field(x => x.FeeWithTax);
            Field(x => x.FeeTotal);
            Field(x => x.FeeTotalWithTax);
            Field(x => x.TaxType);
            Field(x => x.TaxTotal);
            Field(x => x.TaxPercentRate);
            Field(x => x.LanguageCode, true);

            Field<NonNullGraphType<ListGraphType<OrderAddressType>>>(nameof(CustomerOrder.Addresses), resolve: x => x.Source.Addresses);
            Field<NonNullGraphType<ListGraphType<OrderLineItemType>>>(nameof(CustomerOrder.Items), resolve: x => x.Source.Items);
            Field<NonNullGraphType<ListGraphType<PaymentInType>>>(nameof(CustomerOrder.InPayments), resolve: x => x.Source.InPayments);
            Field<NonNullGraphType<ListGraphType<OrderShipmentType>>>(nameof(CustomerOrder.Shipments), resolve: x => x.Source.Shipments);
            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(CustomerOrder.TaxDetails), resolve: x => x.Source.TaxDetails);
            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }
    }
}

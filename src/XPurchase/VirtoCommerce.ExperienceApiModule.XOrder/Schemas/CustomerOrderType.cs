using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class CustomerOrderType : ObjectGraphType<CustomerOrderAggregate> 
    {
        public CustomerOrderType()
        {
            Field(x => x.Order.Id);
            Field(x => x.Order.OperationType);
            Field(x => x.Order.ParentOperationId);
            Field(x => x.Order.Number);
            Field(x => x.Order.IsApproved);
            Field(x => x.Order.Status, true);
            Field(x => x.Order.Comment, true);
            Field(x => x.Order.Currency);
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
            Field(x => x.Order.DiscountAmount);
            Field(x => x.Order.Total);
            Field(x => x.Order.SubTotal);
            Field(x => x.Order.SubTotalWithTax);
            Field(x => x.Order.SubTotalDiscount);
            Field(x => x.Order.SubTotalDiscountWithTax);
            Field(x => x.Order.SubTotalTaxTotal);
            Field(x => x.Order.ShippingTotal);
            Field(x => x.Order.ShippingTotalWithTax);
            Field(x => x.Order.ShippingSubTotal);
            Field(x => x.Order.ShippingSubTotalWithTax);
            Field(x => x.Order.ShippingDiscountTotal);
            Field(x => x.Order.ShippingDiscountTotalWithTax);
            Field(x => x.Order.ShippingTaxTotal);
            Field(x => x.Order.PaymentTotal);
            Field(x => x.Order.PaymentTotalWithTax);
            Field(x => x.Order.PaymentSubTotal);
            Field(x => x.Order.PaymentSubTotalWithTax);
            Field(x => x.Order.PaymentDiscountTotal);
            Field(x => x.Order.PaymentDiscountTotalWithTax);
            Field(x => x.Order.PaymentTaxTotal);
            Field(x => x.Order.DiscountTotal);
            Field(x => x.Order.DiscountTotalWithTax);
            Field(x => x.Order.Fee);
            Field(x => x.Order.FeeWithTax);
            Field(x => x.Order.FeeTotal);
            Field(x => x.Order.FeeTotalWithTax);
            Field(x => x.Order.TaxType);
            Field(x => x.Order.TaxTotal);
            Field(x => x.Order.TaxPercentRate);
            Field(x => x.Order.LanguageCode, true);

            Field<NonNullGraphType<ListGraphType<OrderAddressType>>>(nameof(CustomerOrder.Addresses), resolve: x => x.Source.Order.Addresses);
            Field<NonNullGraphType<ListGraphType<LineItemType>>>(nameof(CustomerOrder.Items), resolve: x => x.Source.Order.Items);
            Field<NonNullGraphType<ListGraphType<PaymentInType>>>(nameof(CustomerOrder.InPayments), resolve: x => x.Source.Order.InPayments);
            Field<NonNullGraphType<ListGraphType<ShipmentType>>>(nameof(CustomerOrder.Shipments), resolve: x => x.Source.Order.Shipments);
            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(CustomerOrder.TaxDetails), resolve: x => x.Source.Order.TaxDetails);
            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }
    }
}

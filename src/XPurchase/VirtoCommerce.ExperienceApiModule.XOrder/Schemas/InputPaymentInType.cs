using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputPaymentInType : InputObjectGraphType<PaymentIn>
    {
        public InputPaymentInType()
        {
            Field(x => x.Id);
            Field(x => x.OuterId, true);
            Field(x => x.OperationType);
            Field(x => x.Number);
            Field(x => x.IsApproved);
            Field(x => x.Status, true);
            Field(x => x.Comment, true);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate, true);
            Field(x => x.CancelReason, true);
            Field(x => x.ParentOperationId, true);
            Field(x => x.ObjectType, true);

            Field(x => x.OrganizationId, true);
            Field(x => x.OrganizationName, true);
            Field(x => x.CustomerName, true);
            Field(x => x.CustomerId);
            Field(x => x.Purpose, true);
            Field(x => x.GatewayCode, true);
            Field(x => x.IncomingDate, true);
            Field(x => x.AuthorizedDate, true);
            Field(x => x.CapturedDate, true);
            Field(x => x.VoidedDate, true);
            Field(x => x.OrderId, true);
            Field(x => x.Sum);
            Field(x => x.TaxTotal);
            Field(x => x.Currency);
            Field<IntGraphType>(nameof(PaymentIn.PaymentStatus));
            //Field(x => x.Price);
            //Field(x => x.PriceWithTax);
            //Field(x => x.Total);
            //Field(x => x.TotalWithTax);
            //Field(x => x.DiscountAmount);
            //Field(x => x.DiscountAmountWithTax);
            //Field(x => x.TaxPercentRate);
            Field(x => x.TaxType, true);

            Field<NonNullGraphType<ListGraphType<InputOrderTaxDetailType>>>(nameof(PaymentIn.TaxDetails));
            Field<NonNullGraphType<ListGraphType<InputOrderDiscountType>>>(nameof(PaymentIn.Discounts));
            Field<InputOrderPaymentMethodType>(nameof(PaymentIn.PaymentMethod));
            Field<InputOrderAddressType>(nameof(PaymentIn.BillingAddress));

            //TODO
            //Transactions
            //public IList<Operation> ChildrenOperations);
            //public IList<DynamicProperty> DynamicProperties);
        }
    }
}

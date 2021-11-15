using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputPaymentInType : InputObjectGraphType<PaymentIn>
    {
        public InputPaymentInType()
        {
            Field(x => x.Id, true);
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
            Field(x => x.OrderId);
            Field(x => x.Sum);
            Field(x => x.TaxTotal);
            Field(x => x.Currency);
            Field<IntGraphType>(nameof(PaymentIn.PaymentStatus),
                "Payment status");
            Field(x => x.TaxType, true);

            Field<ListGraphType<InputOrderTaxDetailType>>(nameof(PaymentIn.TaxDetails),
                "Tax details");
            Field<ListGraphType<InputOrderDiscountType>>(nameof(PaymentIn.Discounts),
                "Discounts");
            Field<InputOrderPaymentMethodType>(nameof(PaymentIn.PaymentMethod),
                "PaymentMethod");
            Field<InputOrderAddressType>(nameof(PaymentIn.BillingAddress),
                "Billing address");

            //PT-5383: Add additional properties to XOrder types:
            //Transactions
            //public IList<Operation> ChildrenOperations);
            //public IList<DynamicProperty> DynamicProperties);
        }
    }
}

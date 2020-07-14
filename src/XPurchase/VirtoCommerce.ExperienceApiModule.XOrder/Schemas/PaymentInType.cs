using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class PaymentInType : ObjectGraphType<PaymentIn>
    {
        public PaymentInType()
        {
            Field(x => x.OrganizationId);
            Field(x => x.CustomerName);
            Field(x => x.CustomerId);
            Field(x => x.Purpose);
            Field(x => x.GatewayCode);
            
            Field(x => x.IncomingDate);
            Field(x => x.OuterId);
            Field(x => x.OperationType);
            Field(x => x.Number);
            Field(x => x.IsApproved);
            Field(x => x.Status);
            Field(x => x.Comment);
            
            Field(x => x.Sum);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate);
            Field(x => x.CancelReason);
            Field(x => x.ParentOperationId);
            Field(x => x.ObjectType);
            Field(x => x.CreatedDate);
            Field(x => x.ModifiedDate);
            Field(x => x.CreatedBy);
            Field(x => x.ModifiedBy);
            Field(x => x.AuthorizedDate);
            Field(x => x.CapturedDate);
            Field(x => x.VoidedDate);
            Field(x => x.OrderId);
            Field(x => x.Id);

            Field<OrderCurrencyType>(nameof(PaymentIn.Currency), resolve: context => context.Source.Currency);
            Field<OrderAddressType>(nameof(PaymentIn.BillingAddress), resolve: context => context.Source.BillingAddress);

            //TODO
            //Field(x => x.TaxIncluded);
            //Field(x => x.PaymentMethodType);
            //public IList<Operation> ChildrenOperations);
            //public BankCardInfo BankCardInfo);
            //public IList<DynamicProperty> DynamicProperties);
        }
    }
}

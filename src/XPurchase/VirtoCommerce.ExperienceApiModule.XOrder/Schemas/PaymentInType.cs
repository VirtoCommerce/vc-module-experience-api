using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class PaymentInType : ObjectGraphType<PaymentIn>
    {
        public PaymentInType()
        {
            Field(x => x.Id);
            Field(x => x.OrganizationId);
            Field(x => x.CustomerName);
            Field(x => x.CustomerId);
            Field(x => x.Purpose);
            Field(x => x.GatewayCode);
            Field(x => x.IncomingDate, true);
            Field(x => x.OuterId);
            Field(x => x.OperationType);
            Field(x => x.Number);
            Field(x => x.IsApproved);
            Field(x => x.Status);
            Field(x => x.Comment);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate, true);
            Field(x => x.CancelReason);
            Field(x => x.ParentOperationId);
            Field(x => x.ObjectType);
            Field(x => x.CreatedDate);
            Field(x => x.ModifiedDate, true);
            Field(x => x.CreatedBy);
            Field(x => x.ModifiedBy);
            Field(x => x.AuthorizedDate, true);
            Field(x => x.CapturedDate, true);
            Field(x => x.VoidedDate, true);
            Field(x => x.OrderId);

            Field<OrderMoneyType>(nameof(PaymentIn.Sum).ToCamelCase(), resolve: context => new Money(context.Source.Sum, context.OrderCurency(context.Source.OrderId)));
            Field<OrderMoneyType>("tax", resolve: context => new Money(context.Source.TaxTotal, context.OrderCurency(context.Source.OrderId)));
            Field<StringGraphType>(nameof(PaymentIn.PaymentMethod), resolve: context => context.Source.PaymentMethod.Code);
            Field<OrderCurrencyType>(nameof(PaymentIn.Currency), resolve: context => context.OrderCurency(context.Source.OrderId));
            Field<OrderAddressType>(nameof(PaymentIn.BillingAddress), resolve: context => context.Source.BillingAddress);

            //TODO
            //public IList<Operation> ChildrenOperations);
            //public IList<DynamicProperty> DynamicProperties);
        }
    }
}

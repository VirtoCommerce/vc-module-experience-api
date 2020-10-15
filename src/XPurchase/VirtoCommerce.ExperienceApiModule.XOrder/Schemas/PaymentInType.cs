using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class PaymentInType : ObjectGraphType<PaymentIn>
    {
        public PaymentInType()
        {
            Field(x => x.Id);
            Field(x => x.OrganizationId, true);
            Field(x => x.OrganizationName, true);
            Field(x => x.CustomerName, true);
            Field(x => x.CustomerId);
            Field(x => x.Purpose, true);
            Field(x => x.GatewayCode, true);
            Field(x => x.IncomingDate, true);
            Field(x => x.OuterId, true);
            Field(x => x.OperationType);
            Field(x => x.Number);
            Field(x => x.IsApproved);
            Field(x => x.Status, true);
            Field(x => x.Comment, true);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate, true);
            Field(x => x.CancelReason, true);
            Field(x => x.ParentOperationId);
            Field(x => x.ObjectType);
            Field(x => x.CreatedDate);
            Field(x => x.ModifiedDate, true);
            Field(x => x.CreatedBy, true);
            Field(x => x.ModifiedBy, true);
            Field(x => x.AuthorizedDate, true);
            Field(x => x.CapturedDate, true);
            Field(x => x.VoidedDate, true);
            Field(x => x.OrderId, true);

            Field<OrderMoneyType>(nameof(PaymentIn.Sum).ToCamelCase(), resolve: context => new Money(context.Source.Sum, context.GetCurrencyByCode(context.Source.Currency)));
            Field<OrderMoneyType>("tax", resolve: context => new Money(context.Source.TaxTotal, context.GetCurrencyByCode(context.Source.Currency)));
            Field<StringGraphType>(nameof(PaymentIn.PaymentMethod), resolve: context => context.Source.PaymentMethod.Code);
            Field<CurrencyType>(nameof(PaymentIn.Currency), resolve: context => context.GetCurrencyByCode(context.Source.Currency));
            Field<OrderAddressType>(nameof(PaymentIn.BillingAddress), resolve: context => context.Source.BillingAddress);

            Field<ListGraphType<OrderAddressType>>(nameof(PaymentIn.Transactions), resolve: x => x.Source.Transactions);
            //TODO
            //public IList<Operation> ChildrenOperations);
            //public IList<DynamicProperty> DynamicProperties);
        }
    }
}

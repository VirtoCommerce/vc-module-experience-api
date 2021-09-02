using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class PaymentInType : ExtendableGraphType<PaymentIn>
    {
        public PaymentInType(IDynamicPropertyResolverService dynamicPropertyResolverService, ICustomerOrderAggregateRepository customerOrderAggregateRepository)
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
            Field(x => x.ParentOperationId, true);
            Field(x => x.ObjectType);
            Field(x => x.CreatedDate);
            Field(x => x.ModifiedDate, true);
            Field(x => x.CreatedBy, true);
            Field(x => x.ModifiedBy, true);
            Field(x => x.AuthorizedDate, true);
            Field(x => x.CapturedDate, true);
            Field(x => x.VoidedDate, true);
            Field(x => x.OrderId, true);

            Field<MoneyType>(nameof(PaymentIn.Price).ToCamelCase(), resolve: context => new Money(context.Source.Price, context.GetOrderCurrencyByCode(context.Source.Currency)));
            Field<MoneyType>(nameof(PaymentIn.Sum).ToCamelCase(), resolve: context => new Money(context.Source.Sum, context.GetOrderCurrencyByCode(context.Source.Currency)));
            Field<MoneyType>("tax", resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrencyByCode(context.Source.Currency)));
            ExtendableField<OrderPaymentMethodType>(nameof(PaymentIn.PaymentMethod), resolve: context => context.Source.PaymentMethod);
            Field<CurrencyType>(nameof(PaymentIn.Currency), resolve: context => context.GetOrderCurrencyByCode(context.Source.Currency));
            ExtendableField<AddressType>(nameof(PaymentIn.BillingAddress), resolve: context => context.Source.BillingAddress);

            Field<ListGraphType<PaymentTransactionType>>(nameof(PaymentIn.Transactions), resolve: x => x.Source.Transactions);
            //TODO
            //public IList<Operation> ChildrenOperations);

            ExtendableField<NonNullGraphType<CustomerOrderType>>(
                "order",
                "Associated Order",
                null,
                context =>
                {
                    if (!string.IsNullOrEmpty(context.Source.OrderId))
                    {
                        return customerOrderAggregateRepository.GetOrderByIdAsync(context.Source.OrderId);
                    }

                    return null;
                }
            );

            ExtendableField<ListGraphType<DynamicPropertyValueType>>(
                "dynamicProperties",
                "Customer order Payment dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetArgumentOrValue<string>("cultureName")));
        }
    }
}

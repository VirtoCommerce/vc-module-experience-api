using AutoMapper;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Settings;
using Money = VirtoCommerce.CoreModule.Core.Currency.Money;
using OrderSettings = VirtoCommerce.OrdersModule.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class PaymentInType : ExtendableGraphType<PaymentIn>
    {
        public PaymentInType(
            IMapper mapper,
            IMemberService memberService,
            IDataLoaderContextAccessor dataLoader,
            IDynamicPropertyResolverService dynamicPropertyResolverService,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            ILocalizableSettingService localizableSettingService)
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.OrganizationId, nullable: true);
            Field(x => x.OrganizationName, nullable: true);
            Field(x => x.CustomerName, nullable: true);
            Field(x => x.CustomerId, nullable: false);
            Field(x => x.Purpose, nullable: true);
            Field(x => x.GatewayCode, nullable: true);
            Field(x => x.IncomingDate, nullable: true);
            Field(x => x.OuterId, nullable: true);
            Field(x => x.OperationType, nullable: false);
            Field(x => x.Number, nullable: false);
            Field(x => x.IsApproved, nullable: false);
            LocalizedField(x => x.Status, OrderSettings.PaymentInStatus, localizableSettingService, nullable: true);
            Field(x => x.Comment, nullable: true);
            Field(x => x.IsCancelled, nullable: false);
            Field(x => x.CancelledDate, nullable: true);
            Field(x => x.CancelReason, nullable: true);
            Field(x => x.ParentOperationId, nullable: true);
            Field(x => x.ObjectType, nullable: false);
            Field(x => x.CreatedDate, nullable: false);
            Field(x => x.ModifiedDate, nullable: true);
            Field(x => x.CreatedBy, nullable: true);
            Field(x => x.ModifiedBy, nullable: true);
            Field(x => x.AuthorizedDate, nullable: true);
            Field(x => x.CapturedDate, nullable: true);
            Field(x => x.VoidedDate, nullable: true);
            Field(x => x.OrderId, nullable: true);

            Field<NonNullGraphType<MoneyType>>(nameof(PaymentIn.Price).ToCamelCase(),
                resolve: context => new Money(context.Source.Price, context.GetOrderCurrencyByCode(context.Source.Currency)));
            Field<NonNullGraphType<MoneyType>>(nameof(PaymentIn.Sum).ToCamelCase(),
                resolve: context => new Money(context.Source.Sum, context.GetOrderCurrencyByCode(context.Source.Currency)));
            Field<NonNullGraphType<MoneyType>>("tax",
                resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrencyByCode(context.Source.Currency)));
            ExtendableField<OrderPaymentMethodType>(nameof(PaymentIn.PaymentMethod), resolve: context => context.Source.PaymentMethod);
            Field<NonNullGraphType<CurrencyType>>(nameof(PaymentIn.Currency), resolve: context => context.GetOrderCurrencyByCode(context.Source.Currency));
            ExtendableField<OrderAddressType>(nameof(PaymentIn.BillingAddress), resolve: context => context.Source.BillingAddress);

            var vendorField = new FieldType
            {
                Name = "vendor",
                Type = GraphTypeExtenstionHelper.GetActualType<VendorType>(),
                Resolver = new FuncFieldResolver<PaymentIn, IDataLoaderResult<ExpVendor>>(context =>
                {
                    return dataLoader.LoadVendor(memberService, mapper, loaderKey: "order_vendor", vendorId: context.Source.VendorId);
                })
            };
            AddField(vendorField);

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<PaymentTransactionType>>>>(nameof(PaymentIn.Transactions), resolve: x => x.Source.Transactions);
            //PT-5383: Add additional properties to XOrder types:
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

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<DynamicPropertyValueType>>>>(
                "dynamicProperties",
                "Customer order Payment dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetCultureName()));
        }
    }
}

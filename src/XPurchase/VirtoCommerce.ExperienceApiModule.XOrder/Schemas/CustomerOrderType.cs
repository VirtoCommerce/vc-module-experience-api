using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class CustomerOrderType : ExtendableGraphType<CustomerOrderAggregate>
    {
        public CustomerOrderType(
            IDynamicPropertyResolverService dynamicPropertyResolverService,
            IPaymentMethodsSearchService paymentMethodsSearchService)
        {
            Field(x => x.Order.Id, nullable: false);
            Field(x => x.Order.OperationType, nullable: false);
            Field(x => x.Order.ParentOperationId, nullable: true);
            Field(x => x.Order.Number, nullable: false);
            Field(x => x.Order.IsApproved, nullable: false);
            Field(x => x.Order.Status, nullable: true);
            Field(x => x.Order.Comment, nullable: true);
            Field(x => x.Order.OuterId, nullable: true);
            Field(x => x.Order.IsCancelled, nullable: false);
            Field(x => x.Order.CancelledDate, nullable: true);
            Field(x => x.Order.CancelReason, nullable: true);
            Field(x => x.Order.ObjectType, nullable: false);
            Field(x => x.Order.CustomerId, nullable: false);
            Field(x => x.Order.CustomerName, nullable: true);
            Field(x => x.Order.ChannelId, nullable: true);
            Field(x => x.Order.StoreId, nullable: false);
            Field(x => x.Order.StoreName, nullable: true);
            Field(x => x.Order.OrganizationId, nullable: true);
            Field(x => x.Order.OrganizationName, nullable: true);
            Field(x => x.Order.EmployeeId, nullable: true);
            Field(x => x.Order.EmployeeName, nullable: true);
            Field(x => x.Order.ShoppingCartId, nullable: true);
            Field(x => x.Order.IsPrototype, nullable: false);
            Field(x => x.Order.SubscriptionNumber, nullable: true);
            Field(x => x.Order.SubscriptionId, nullable: true);
            Field(x => x.Order.PurchaseOrderNumber, nullable: true);
            Field(x => x.Order.TaxType, nullable: true);
            Field(x => x.Order.TaxPercentRate, nullable: false);
            Field(x => x.Order.LanguageCode, nullable: true);
            Field(x => x.Order.CreatedDate, nullable: false);
            Field(x => x.Order.CreatedBy, nullable: true);
            Field(x => x.Order.ModifiedDate, nullable: true);
            Field(x => x.Order.ModifiedBy, nullable: true);

            Field<NonNullGraphType<CurrencyType>>(nameof(CustomerOrder.Currency).ToCamelCase(),
                resolve: context => context.Source.Currency);
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.Total).ToCamelCase(),
                resolve: context => new Money(context.Source.Order.Total, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.TaxTotal).ToCamelCase(),
                resolve: context => new Money(context.Source.Order.TaxTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.DiscountAmount).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.DiscountAmount, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.SubTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.SubTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.SubTotalWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.SubTotalWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.SubTotalDiscount).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.SubTotalDiscount, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.SubTotalDiscountWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.SubTotalDiscountWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.SubTotalTaxTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.SubTotalTaxTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.ShippingTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.ShippingTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.ShippingTotalWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.ShippingTotalWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.ShippingSubTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.ShippingSubTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.ShippingSubTotalWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.ShippingSubTotalWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.ShippingDiscountTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.ShippingDiscountTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.ShippingDiscountTotalWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.ShippingDiscountTotalWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.ShippingTaxTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.ShippingTaxTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.PaymentTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.PaymentTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.PaymentTotalWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.PaymentTotalWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.PaymentSubTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.PaymentSubTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.PaymentSubTotalWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.PaymentSubTotalWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.PaymentDiscountTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.PaymentDiscountTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.PaymentDiscountTotalWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.PaymentDiscountTotalWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.PaymentTaxTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.PaymentTaxTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.DiscountTotal).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.DiscountTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.DiscountTotalWithTax).ToCamelCase(),
                 resolve: context => new Money(context.Source.Order.DiscountTotalWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.Fee).ToCamelCase(),
                resolve: context => new Money(context.Source.Order.Fee, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.FeeWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.Order.FeeWithTax, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.FeeTotal).ToCamelCase(),
                resolve: context => new Money(context.Source.Order.FeeTotal, context.Source.Currency));
            Field<NonNullGraphType<MoneyType>>(nameof(CustomerOrder.FeeTotalWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.Order.FeeTotalWithTax, context.Source.Currency));

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<OrderAddressType>>>>(nameof(CustomerOrder.Addresses),
                resolve: x => x.Source.Order.Addresses);
            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<OrderLineItemType>>>>(nameof(CustomerOrder.Items),
                resolve: x => x.Source.Order.Items);
            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<PaymentInType>>>>(nameof(CustomerOrder.InPayments),
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "after" },
                    new QueryArgument<IntGraphType> { Name = "first" },
                    new QueryArgument<StringGraphType> { Name = "sort" }
                ),
                resolve: x =>
                {
                    var skip = x.GetArgument("after", 0);
                    var take = x.GetArgument("first", 20);
                    var sort = x.GetArgument<string>("sort");

                    var payments = x.Source.Order.InPayments ?? new List<PaymentIn>();
                    var queryable = payments.AsQueryable();

                    if (!string.IsNullOrEmpty(sort))
                    {
                        var sortInfos = SortInfo.Parse(sort).ToList();
                        queryable = queryable.OrderBySortInfos(sortInfos);
                    }

                    queryable = queryable.Skip(skip).Take(take);

                    var result = queryable.ToList();
                    return result;
                });

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<OrderShipmentType>>>>(nameof(CustomerOrder.Shipments), resolve: x => x.Source.Order.Shipments);

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<OrderTaxDetailType>>>>(nameof(CustomerOrder.TaxDetails), resolve: x => x.Source.Order.TaxDetails);

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<DynamicPropertyValueType>>>>(
                "dynamicProperties",
                "Customer order dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source.Order, context.GetArgumentOrValue<string>("cultureName")));

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("coupons", resolve: x => x.Source.GetCustomerOrderCoupons());

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<OrderDiscountType>>>>("discounts", resolve: x => x.Source.Order.Discounts);

            ExtendableFieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<OrderPaymentMethodType>>>>("availablePaymentMethods",
                "Available payment methods",
                resolve: async context =>
                {
                    var criteria = new PaymentMethodsSearchCriteria
                    {
                        IsActive = true,
                        StoreId = context.Source.Order.StoreId,
                    };

                    var result = await paymentMethodsSearchService.SearchAsync(criteria);

                    result.Results?.Apply(x => context.UserContext[x.Id] = context.Source);

                    return result.Results;
                });
        }
    }
}

using System.Linq;
using AutoMapper;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using Money = VirtoCommerce.CoreModule.Core.Currency.Money;
using OrderSettings = VirtoCommerce.OrdersModule.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderLineItemType : ExtendableGraphType<LineItem>
    {
        public OrderLineItemType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader,
            IDynamicPropertyResolverService dynamicPropertyResolverService,
            IMapper mapper,
            IMemberService memberService,
            ILocalizableSettingService localizableSettingService)
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.ProductType, nullable: true);
            Field(x => x.Name, nullable: false);
            Field(x => x.Comment, nullable: true);
            Field(x => x.ImageUrl, nullable: true);
            Field(x => x.IsGift, nullable: true);
            Field(x => x.ShippingMethodCode, nullable: true);
            Field(x => x.FulfillmentLocationCode, nullable: true);
            Field(x => x.FulfillmentCenterId, nullable: true);
            Field(x => x.FulfillmentCenterName, nullable: true);
            Field(x => x.OuterId, nullable: true);
            Field(x => x.ProductOuterId, nullable: true);
            Field(x => x.WeightUnit, nullable: true);
            Field(x => x.Weight, nullable: true);
            Field(x => x.MeasureUnit, nullable: true);
            Field(x => x.Height, nullable: true);
            Field(x => x.Length, nullable: true);
            Field(x => x.Width, nullable: true);
            Field(x => x.IsCancelled, nullable: false);
            Field(x => x.CancelledDate, nullable: true);
            Field(x => x.CancelReason, nullable: true);
            Field(x => x.ObjectType, nullable: false);
            LocalizedField(x => x.Status, OrderSettings.OrderLineItemStatuses, localizableSettingService, nullable: true);

            Field(x => x.CategoryId, nullable: true);
            Field(x => x.CatalogId, nullable: false);

            Field(x => x.Sku, nullable: false);
            Field(x => x.PriceId, nullable: true);
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.Price).ToCamelCase(),
                resolve: context => new Money(context.Source.Price, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.PriceWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.PriceWithTax, context.GetOrderCurrency()));
            Field(x => x.TaxType, nullable: true);
            Field(x => x.TaxPercentRate, nullable: false);
            Field(x => x.ReserveQuantity, nullable: false);
            Field(x => x.Quantity, nullable: false);
            Field(x => x.ProductId, nullable: false);

            Field<NonNullGraphType<CurrencyType>>(nameof(LineItem.Currency).ToCamelCase(),
                resolve: context => context.GetOrderCurrency());
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.DiscountAmount).ToCamelCase(),
                resolve: context => new Money(context.Source.DiscountAmount, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.DiscountAmountWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.DiscountTotal).ToCamelCase(),
                resolve: context => new Money(context.Source.DiscountTotal, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.DiscountTotalWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.DiscountTotalWithTax, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.ExtendedPrice).ToCamelCase(),
                resolve: context => new Money(context.Source.ExtendedPrice, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.ExtendedPriceWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.ExtendedPriceWithTax, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.PlacedPrice).ToCamelCase(),
                resolve: context => new Money(context.Source.PlacedPrice, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.PlacedPriceWithTax).ToCamelCase(),
                resolve: context => new Money(context.Source.PlacedPriceWithTax, context.GetOrderCurrency()));
            Field<NonNullGraphType<MoneyType>>(nameof(LineItem.TaxTotal).ToCamelCase(),
                resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrency()));
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<OrderTaxDetailType>>>>(nameof(LineItem.TaxDetails),
                resolve: x => x.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<OrderDiscountType>>>>(nameof(LineItem.Discounts),
                resolve: x => x.Source.Discounts);

            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new FuncFieldResolver<LineItem, IDataLoaderResult<ExpProduct>>(context =>
                {
                    var includeFields = context.SubFields.Values.GetAllNodesPaths(context).ToArray();
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("order_lineItems_products", async (ids) =>
                    {
                        //Get currencies and store only from one order.
                        //We intentionally ignore the case when there are ma be the orders with the different currencies and stores in the resulting set
                        var order = context.GetValueForSource<CustomerOrderAggregate>().Order;
                        var request = new LoadProductsQuery
                        {
                            UserId = context.GetArgumentOrValue<string>("userId") ?? context.GetCurrentUserId(),
                            StoreId = order.StoreId,
                            CurrencyCode = order.Currency,
                            ObjectIds = ids.ToArray(),
                            IncludeFields = includeFields.ToArray()
                        };
                        if (!context.UserContext.ContainsKey("storeId"))
                        {
                            context.UserContext.Add("storeId", order.StoreId);
                        }

                        var response = await mediator.Send(request);

                        return response.Products.ToDictionary(x => x.Id);
                    });

                    return loader.LoadAsync(context.Source.ProductId);
                })
            };
            AddField(productField);

            var vendorField = new FieldType
            {
                Name = "vendor",
                Type = GraphTypeExtenstionHelper.GetActualType<VendorType>(),
                Resolver = new FuncFieldResolver<LineItem, IDataLoaderResult<ExpVendor>>(context =>
                {
                    return dataLoader.LoadVendor(memberService, mapper, loaderKey: "order_vendor", vendorId: context.Source.VendorId);
                })
            };
            AddField(vendorField);

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<DynamicPropertyValueType>>>>(
                "dynamicProperties",
                "Customer order Line item dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetCultureName()));
        }
    }
}

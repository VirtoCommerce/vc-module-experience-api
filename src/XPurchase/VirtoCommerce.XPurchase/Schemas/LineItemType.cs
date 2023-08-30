using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class LineItemType : ExtendableGraphType<LineItem>
    {
        public LineItemType(IMediator mediator, IDataLoaderContextAccessor dataLoader, IDynamicPropertyResolverService dynamicPropertyResolverService, IMapper mapper, IMemberService memberService, ICurrencyService currencyService)
        {
            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new FuncFieldResolver<LineItem, IDataLoaderResult<ExpProduct>>(context =>
                {
                    var includeFields = context.SubFields.Values.GetAllNodesPaths();
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("order_lineItems_products", async (ids) =>
                    {
                        //Get currencies and store only from one cart.
                        //We intentionally ignore the case when there are ma be the carts with the different currencies and stores in the resulting set
                        var cart = context.GetValueForSource<CartAggregate>().Cart;
                        var userId = context.GetArgumentOrValue<string>("userId") ?? cart.CustomerId;

                        var request = new LoadProductsQuery
                        {
                            StoreId = cart.StoreId,
                            CurrencyCode = cart.Currency,
                            ObjectIds = ids.ToArray(),
                            IncludeFields = includeFields.ToArray(),
                            UserId = userId,
                        };

                        var allCurrencies = await currencyService.GetAllCurrenciesAsync();
                        var cultureName = context.GetArgumentOrValue<string>("cultureName") ?? cart.LanguageCode;
                        context.SetCurrencies(allCurrencies, cultureName);
                        context.UserContext.TryAdd("currencyCode", cart.Currency);
                        context.UserContext.TryAdd("storeId", cart.StoreId);

                        var response = await mediator.Send(request);

                        return response.Products.ToDictionary(x => x.Id);
                    });
                    return loader.LoadAsync(context.Source.ProductId);
                })
            };
            AddField(productField);

            //Field<MoneyType>("paymentPlan", resolve: context => context.Source.PaymentPlan);
            Field<IntGraphType>("inStockQuantity",
                "In stock quantity",
                resolve: context => context.GetCart().CartProducts[context.Source.ProductId]?.AvailableQuantity ?? 0);
            Field<StringGraphType>("warehouseLocation",
                "Warehouse location",
                resolve: context => context.GetCart().CartProducts[context.Source.ProductId]?.Inventory?.FulfillmentCenter?.Address?.ToString());

            Field<BooleanGraphType>("IsValid",
                "Shows whether this is valid",
                resolve: context => !context.GetCart().ValidationErrors.GetEntityCartErrors(context.Source).Any());
            Field<ListGraphType<ValidationErrorType>>("validationErrors",
                "Validation errors",
                resolve: context => context.GetCart().ValidationErrors.GetEntityCartErrors(context.Source));

            Field(x => x.CatalogId, nullable: true).Description("Catalog ID value");
            Field(x => x.CategoryId, nullable: true).Description("Category ID value");
            Field(x => x.CreatedDate, nullable: true).Description("Line item create date");
            Field(x => x.Height, nullable: true).Description("Height value");
            Field(x => x.Id, nullable: false).Description("Line item ID");
            Field(x => x.ImageUrl, nullable: true).Description("Value of line item image absolute URL");
            Field(x => x.IsGift, nullable: true).Description("flag of line item is a gift");
            Field(x => x.IsReadOnly, nullable: true).Description("Shows whether this is read-only");
            Field(x => x.IsReccuring, nullable: true).Description("Shows whether the line item is recurring");
            Field(x => x.LanguageCode, nullable: true).Description("Culture name in the ISO 3166-1 alpha-3 format");
            Field(x => x.Length, nullable: true).Description("Length value");
            Field(x => x.MeasureUnit, nullable: true).Description("Measurement unit value");
            Field(x => x.Name, nullable: true).Description("Line item name value");
            Field(x => x.Note, nullable: true).Description("Line item comment value");
            Field(x => x.ObjectType, nullable: true).Description("Line item quantity value");
            Field(x => x.ProductId, nullable: true).Description("Product ID value");
            Field(x => x.ProductType, nullable: true).Description("Product type: Physical, Digital, or Subscription");
            Field(x => x.Quantity, nullable: true).Description("Line item quantity value");
            Field(x => x.RequiredShipping, nullable: true).Description("Requirement for line item shipping");
            Field(x => x.ShipmentMethodCode, nullable: true).Description("Line item shipping method code value");
            Field(x => x.Sku, nullable: true).Description("Product SKU value");
            Field(x => x.TaxPercentRate, nullable: true).Description("Total shipping tax amount value");
            Field(x => x.TaxType, nullable: true).Description("Shipping tax type value");
            Field(x => x.ThumbnailImageUrl, nullable: true).Description("Value of line item thumbnail image absolute URL");
            Field(x => x.VolumetricWeight, nullable: true).Description("Volumetric weight value");
            Field(x => x.Weight, nullable: true).Description("Shopping cart weight value");
            Field(x => x.WeightUnit, nullable: true).Description("Weight unit value");
            Field(x => x.Width, nullable: true).Description("Width value");
            Field(x => x.FulfillmentCenterId, nullable: true).Description("Line item fulfillment center ID value");
            Field(x => x.FulfillmentCenterName, nullable: true).Description("Line item fulfillment center name value");
            Field<ListGraphType<DiscountType>>("discounts",
                "Discounts",
                resolve: context => context.Source.Discounts);
            Field<ListGraphType<TaxDetailType>>("taxDetails",
                "Tax details",
                resolve: context => context.Source.TaxDetails);
            Field<MoneyType>("discountAmount",
                "Discount amount",
                resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountAmountWithTax",
                "Discount amount with tax",
                resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountTotal",
                "Total discount",
                resolve: context => context.Source.DiscountTotal.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountTotalWithTax",
                "Total discount with tax",
                resolve: context => context.Source.DiscountTotalWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("extendedPrice",
                "Extended price",
                resolve: context => context.Source.ExtendedPrice.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("extendedPriceWithTax",
                "Extended price with tax",
                resolve: context => context.Source.ExtendedPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("listPrice",
                "List price",
                resolve: context => context.Source.ListPrice.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("listPriceWithTax",
                "List price with tax",
                resolve: context => context.Source.ListPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("placedPrice",
                "Placed price",
                resolve: context => context.Source.PlacedPrice.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("placedPriceWithTax",
                "Placed price with tax",
                resolve: context => context.Source.PlacedPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("salePrice",
                "Sale price",
                resolve: context => context.Source.SalePrice.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("salePriceWithTax",
                "Sale price with tax",
                resolve: context => context.Source.SalePriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("taxTotal",
                "Tax total",
                resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));

            ExtendableField<ListGraphType<DynamicPropertyValueType>>(
                "dynamicProperties",
                "Cart line item dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetArgumentOrValue<string>("cultureName")));
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class LineItemType : ObjectGraphType<LineItem>
    {
        public LineItemType(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new AsyncFieldResolver<LineItem, object>(async context =>
                {
                    var includeFields = context.SubFields.Values.GetAllNodesPaths(); 
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("order_lineItems_products", async (ids) =>
                    {
                        //Get currencies and store only from one cart.
                        //We intentionally ignore the case when there are ma be the carts with the different currencies and stores in the resulting set
                        var cart = context.GetValueForSource<CartAggregate>().Cart;
                        var request = new LoadProductsQuery
                        {
                            StoreId = cart.StoreId,
                            CurrencyCode = cart.Currency,
                            ObjectIds = ids.ToArray(),
                            IncludeFields = includeFields.ToArray()
                        };

                        var response = await mediator.Send(request);

                        return response.Products.ToDictionary(x => x.Id);
                    });

                    // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
                    var loadHandle = loader.LoadAsync(context.Source.ProductId).GetResultAsync();
                    return await loadHandle;
                })
            };
            AddField(productField);

            //Field<MoneyType>("paymentPlan", resolve: context => context.Source.PaymentPlan);
            Field<IntGraphType>("inStockQuantity", resolve: context => context.GetCart().CartProducts[context.Source.ProductId]?.AvailableQuantity ?? 0);
            Field<StringGraphType>("warehouseLocation", resolve: context => context.GetCart().CartProducts[context.Source.ProductId]?.Inventory?.FulfillmentCenter?.Address);

            Field<BooleanGraphType>("IsValid", resolve: context => !context.GetCart().ValidationErrors.GetEntityCartErrors(context.Source).Any());
            Field<ListGraphType<ValidationErrorType>>("validationErrors", resolve: context => context.GetCart().ValidationErrors.GetEntityCartErrors(context.Source));

            Field(x => x.CatalogId, nullable: true).Description("Value of catalog id");
            Field(x => x.CategoryId, nullable: true).Description("Value of category id");
            Field(x => x.CreatedDate, nullable: true).Description("Line item created date");
            Field(x => x.Height, nullable: true).Description("Value of height");
            Field(x => x.Id, nullable: false).Description("Line item id");
            Field(x => x.ImageUrl, nullable: true).Description("Value of line item image absolute URL");
            Field(x => x.IsGift, nullable: true).Description("flag of line item is a gift");
            Field(x => x.IsReadOnly, nullable: true).Description("Is readOnly");
            Field(x => x.IsReccuring, nullable: true).Description("flag of line item is recurring");
            Field(x => x.LanguageCode, nullable: true).Description("Culture name in ISO 3166-1 alpha-3 format");
            Field(x => x.Length, nullable: true).Description("Value of length");
            Field(x => x.MeasureUnit, nullable: true).Description("Value of measurement unit");
            Field(x => x.Name, nullable: true).Description("Value of line item name");
            Field(x => x.Note, nullable: true).Description("Value of line item comment");
            Field(x => x.ObjectType, nullable: true).Description("Value of line item quantity");
            Field(x => x.ProductId, nullable: true).Description("Value of product id");
            Field(x => x.ProductType, nullable: true).Description("type of product (can be Physical, Digital or Subscription)");
            Field(x => x.Quantity, nullable: true).Description("Value of line item quantity");
            Field(x => x.RequiredShipping, nullable: true).Description("requirement for line item shipping");
            Field(x => x.ShipmentMethodCode, nullable: true).Description("Value of line item shipping method code");
            Field(x => x.Sku, nullable: true).Description("Value of product SKU");
            Field(x => x.TaxPercentRate, nullable: true).Description("Value of total shipping tax amount");
            Field(x => x.TaxType, nullable: true).Description("Value of shipping tax type");
            Field(x => x.ThumbnailImageUrl, nullable: true).Description("Value of line item thumbnail image absolute URL");
            Field(x => x.VolumetricWeight, nullable: true).Description("Value of volumetric weight");
            Field(x => x.Weight, nullable: true).Description("Value of shopping cart weight");
            Field(x => x.WeightUnit, nullable: true).Description("Value of weight unit");
            Field(x => x.Width, nullable: true).Description("Value of width");
            Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            Field<MoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountTotal", resolve: context => context.Source.DiscountTotal.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountTotalWithTax", resolve: context => context.Source.DiscountTotalWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("extendedPrice", resolve: context => context.Source.ExtendedPrice.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("extendedPriceWithTax", resolve: context => context.Source.ExtendedPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("listPrice", resolve: context => context.Source.ListPrice.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("listPriceWithTax", resolve: context => context.Source.ListPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("placedPrice", resolve: context => context.Source.PlacedPrice.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("placedPriceWithTax", resolve: context => context.Source.PlacedPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("salePrice", resolve: context => context.Source.SalePrice.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("salePriceWithTax", resolve: context => context.Source.SalePriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("taxTotal", resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));
        }
    }
}

using System.Linq;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderLineItemType : ObjectGraphType<LineItem>
    {
        public OrderLineItemType(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            Field(x => x.Id);
            Field(x => x.ProductType, true);
            Field(x => x.Name);
            Field(x => x.Comment, true);
            Field(x => x.ImageUrl, true);
            Field(x => x.IsGift, true);
            Field(x => x.ShippingMethodCode, true);
            Field(x => x.FulfillmentLocationCode, true);
            Field(x => x.FulfillmentCenterId, true);
            Field(x => x.FulfillmentCenterName, true);
            Field(x => x.OuterId, true);
            Field(x => x.WeightUnit, true);
            Field(x => x.Weight, true);
            Field(x => x.MeasureUnit, true);
            Field(x => x.Height, true);
            Field(x => x.Length, true);
            Field(x => x.Width, true);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate, true);
            Field(x => x.CancelReason, true);
            Field(x => x.ObjectType);
            
            Field(x => x.CategoryId, true);
            Field(x => x.CatalogId);
            
            Field(x => x.Sku);
            Field(x => x.PriceId, true);
            Field(x => x.Price);
            Field(x => x.PriceWithTax);
            Field(x => x.TaxType, true);
            Field(x => x.TaxPercentRate);
            Field(x => x.ReserveQuantity);
            Field(x => x.Quantity);
            Field(x => x.ProductId);

            Field<CurrencyType>(nameof(LineItem.Currency).ToCamelCase(), resolve: context => context.GetOrderCurrency());
            Field<OrderMoneyType>(nameof(LineItem.DiscountAmount).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmount, context.GetOrderCurrency()));
            Field<OrderMoneyType>(nameof(LineItem.DiscountAmountWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrderCurrency()));
            Field<OrderMoneyType>(nameof(LineItem.DiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.DiscountTotal, context.GetOrderCurrency()));
            Field<OrderMoneyType>(nameof(LineItem.DiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountTotalWithTax, context.GetOrderCurrency()));
            Field<OrderMoneyType>(nameof(LineItem.ExtendedPrice).ToCamelCase(), resolve: context => new Money(context.Source.ExtendedPrice, context.GetOrderCurrency()));
            Field<OrderMoneyType>(nameof(LineItem.ExtendedPriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.ExtendedPriceWithTax, context.GetOrderCurrency()));
            Field<OrderMoneyType>(nameof(LineItem.PlacedPrice).ToCamelCase(), resolve: context => new Money(context.Source.PlacedPrice, context.GetOrderCurrency()));
            Field<OrderMoneyType>(nameof(LineItem.PlacedPriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.PlacedPriceWithTax, context.GetOrderCurrency()));
            Field<OrderMoneyType>(nameof(LineItem.TaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrency()));
            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(LineItem.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<OrderDiscountType>>>(nameof(LineItem.Discounts), resolve: x => x.Source.Discounts);

            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new AsyncFieldResolver<LineItem, object>(async context =>
                {
                    var includeFields = context.SubFields.Values.GetAllNodesPaths();
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("order_lineItems_products", async (ids) =>
                    {
                        //Get currencies and store only from one order.
                        //We intentionally ignore the case when there are ma be the orders with the different currencies and stores in the resulting set
                        var order = context.GetValueForSource<CustomerOrderAggregate>().Order;
                        var request = new LoadProductsQuery
                        {
                            StoreId = order.StoreId,
                            CurrencyCode = order.Currency,
                            ObjectIds = ids.ToArray(),
                            IncludeFields = includeFields.ToArray()
                        };

                        var response = await mediator.Send(request);

                        return response.Products.ToDictionary(x => x.Id);
                    });

                    // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
                    var loadHandle = loader.LoadAsync(context.Source.ProductId);
                    return await loadHandle;
                })
            };
            AddField(productField);
            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }
    }
}

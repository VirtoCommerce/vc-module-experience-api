using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderLineItemType : ObjectGraphType<LineItem>
    {
        public OrderLineItemType()
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
            Field<MoneyType>(nameof(LineItem.DiscountAmount).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmount, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(LineItem.DiscountAmountWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(LineItem.DiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.DiscountTotal, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(LineItem.DiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountTotalWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(LineItem.ExtendedPrice).ToCamelCase(), resolve: context => new Money(context.Source.ExtendedPrice, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(LineItem.ExtendedPriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.ExtendedPriceWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(LineItem.PlacedPrice).ToCamelCase(), resolve: context => new Money(context.Source.PlacedPrice, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(LineItem.PlacedPriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.PlacedPriceWithTax, context.GetOrderCurrency()));
            Field<MoneyType>(nameof(LineItem.TaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.TaxTotal, context.GetOrderCurrency()));
            Field<ListGraphType<OrderTaxDetailType>>(nameof(LineItem.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field<ListGraphType<OrderDiscountType>>(nameof(LineItem.Discounts), resolve: x => x.Source.Discounts);

            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }
    }
}

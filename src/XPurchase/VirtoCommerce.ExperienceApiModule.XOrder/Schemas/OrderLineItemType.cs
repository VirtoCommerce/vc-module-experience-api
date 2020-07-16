using GraphQL;
using GraphQL.NewtonsoftJson;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderLineItemType : ObjectGraphType<LineItem>
    {
        public OrderLineItemType()
        {
            Field(x => x.Id);
            Field(x => x.ProductType);
            Field(x => x.Name);
            Field(x => x.Comment);
            Field(x => x.ImageUrl);
            Field(x => x.IsGift, true);
            Field(x => x.ShippingMethodCode);
            Field(x => x.FulfillmentLocationCode);
            Field(x => x.FulfillmentCenterId);
            Field(x => x.FulfillmentCenterName);
            Field(x => x.OuterId);
            Field(x => x.WeightUnit);
            Field(x => x.Weight, true);
            Field(x => x.MeasureUnit);
            Field(x => x.Height, true);
            Field(x => x.Length, true);
            Field(x => x.Width, true);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate, true);
            Field(x => x.CancelReason);
            Field(x => x.ObjectType);
            
            Field(x => x.CategoryId);
            Field(x => x.CatalogId);
            
            Field(x => x.Sku);
            Field(x => x.PriceId);
            Field(x => x.Currency);
            Field(x => x.Price);
            Field(x => x.PriceWithTax);
            Field(x => x.Fee);
            Field(x => x.FeeWithTax);
            Field(x => x.TaxType);
            Field(x => x.TaxPercentRate);
            Field(x => x.ReserveQuantity);
            Field(x => x.Quantity);
            Field(x => x.ProductId);

            //Field<OrderCurrencyType>(nameof(LineItem.Currency).ToCamelCase(), resolve: context => context.GetOrder().Currency);
            Field<OrderMoneyType>(nameof(LineItem.DiscountAmount).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmount, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(LineItem.DiscountAmountWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountAmountWithTax, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(LineItem.DiscountTotal).ToCamelCase(), resolve: context => new Money(context.Source.DiscountTotal, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(LineItem.DiscountTotalWithTax).ToCamelCase(), resolve: context => new Money(context.Source.DiscountTotalWithTax, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(LineItem.ExtendedPrice).ToCamelCase(), resolve: context => new Money(context.Source.ExtendedPrice, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(LineItem.ExtendedPriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.ExtendedPriceWithTax, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(LineItem.PlacedPrice).ToCamelCase(), resolve: context => new Money(context.Source.PlacedPrice, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(LineItem.PlacedPriceWithTax).ToCamelCase(), resolve: context => new Money(context.Source.PlacedPriceWithTax, context.GetOrder().Currency));
            Field<OrderMoneyType>(nameof(LineItem.TaxTotal).ToCamelCase(), resolve: context => new Money(context.Source.TaxTotal, context.GetOrder().Currency));
            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(LineItem.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<OrderDiscountType>>>(nameof(LineItem.Discounts), resolve: x => x.Source.Discounts);

            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }
    }
}

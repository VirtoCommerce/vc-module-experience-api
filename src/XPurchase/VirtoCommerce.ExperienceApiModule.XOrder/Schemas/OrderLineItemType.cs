using GraphQL.Types;
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
            Field(x => x.PlacedPrice);
            Field(x => x.PlacedPriceWithTax);
            Field(x => x.ExtendedPrice);
            Field(x => x.ExtendedPriceWithTax);
            Field(x => x.DiscountAmount);
            
            Field(x => x.DiscountAmountWithTax);
            Field(x => x.DiscountTotalWithTax);
            Field(x => x.Fee);
            Field(x => x.FeeWithTax);
            Field(x => x.TaxType);
            Field(x => x.TaxTotal);
            Field(x => x.TaxPercentRate);
            Field(x => x.ReserveQuantity);
            Field(x => x.Quantity);
            Field(x => x.ProductId);
            Field(x => x.DiscountTotal);

            Field<NonNullGraphType<ListGraphType<OrderTaxDetailType>>>(nameof(LineItem.TaxDetails), resolve: x => x.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<OrderDiscountType>>>(nameof(LineItem.Discounts), resolve: x => x.Source.Discounts);

            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }
    }
}

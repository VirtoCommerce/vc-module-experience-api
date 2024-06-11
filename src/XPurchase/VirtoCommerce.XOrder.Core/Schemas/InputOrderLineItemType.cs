using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class InputOrderLineItemType : InputObjectGraphType<LineItem>
    {
        public InputOrderLineItemType()
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

            Field(x => x.Currency);
            Field(x => x.DiscountAmount);
            Field(x => x.DiscountAmountWithTax);
            Field(x => x.DiscountTotal);
            Field(x => x.DiscountTotalWithTax);
            Field(x => x.ExtendedPrice);
            Field(x => x.ExtendedPriceWithTax);
            Field(x => x.PlacedPrice);
            Field(x => x.PlacedPriceWithTax);
            Field(x => x.TaxTotal);
            Field<NonNullGraphType<ListGraphType<InputOrderTaxDetailType>>>(nameof(LineItem.TaxDetails),
                "Tax details");
            Field<NonNullGraphType<ListGraphType<InputOrderDiscountType>>>(nameof(LineItem.Discounts),
                "Discounts");

            //PT-5383
            //public ICollection<DynamicObjectProperty> DynamicProperties);
        }
    }
}

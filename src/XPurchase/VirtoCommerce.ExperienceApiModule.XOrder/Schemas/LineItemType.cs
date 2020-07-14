using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class LineItemType : ObjectGraphType<LineItem>
    {
        public LineItemType()
        {
            Field(x => x.Id);
            Field(x => x.ProductType);
            Field(x => x.Name);
            Field(x => x.Comment);
            Field(x => x.ImageUrl);
            Field(x => x.IsGift);
            Field(x => x.ShippingMethodCode);
            Field(x => x.FulfillmentLocationCode);
            Field(x => x.FulfillmentCenterId);
            Field(x => x.FulfillmentCenterName);
            Field(x => x.OuterId);
            Field(x => x.WeightUnit);
            Field(x => x.Weight);
            Field(x => x.MeasureUnit);
            Field(x => x.Height);
            Field(x => x.Length);
            Field(x => x.Width);
            Field(x => x.IsCancelled);
            Field(x => x.CancelledDate);
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

            //TODO
            //public ICollection<DynamicObjectProperty> DynamicProperties);
            //public ICollection<TaxDetail> TaxDetails);
            //public ICollection<Discount> Discounts);
        }
    }
}

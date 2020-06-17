using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class LineItemType : ObjectGraphType<LineItem>
    {
        public LineItemType()
        {
            Field(x => x.CreatedDate, nullable: false).Description("Line item created date");
            Field<ObjectGraphType<ProductType>>("product", resolve: context => context.Source.Product);
            Field(x => x.ProductId, nullable: true).Description("value of product id");
            Field(x => x.ProductType, nullable: true).Description("type of product (can be Physical, Digital or Subscription)");
            Field(x => x.CatalogId, nullable: true).Description("value of catalog id");
            Field(x => x.CategoryId, nullable: true).Description("value of category id");
            Field(x => x.Sku, nullable: true).Description("value of product SKU");
            Field(x => x.Name, nullable: true).Description("value of line item name");
            Field(x => x.Quantity, nullable: true).Description("value of line item quantity");
            Field(x => x.InStockQuantity, nullable: true).Description("InStockQuantity");
            Field(x => x.WarehouseLocation, nullable: true).Description("value of line item warehouse location");
            Field(x => x.ShipmentMethodCode, nullable: true).Description("value of line item shipping method code");
            Field(x => x.RequiredShipping, nullable: true).Description("requirement for line item shipping");
            Field(x => x.ThumbnailImageUrl, nullable: true).Description("value of line item thumbnail image absolute URL");
            Field(x => x.ImageUrl, nullable: true).Description("value of line item image absolute URL");
            Field(x => x.IsGift, nullable: true).Description("flag of line item is a gift");
            Field(x => x.LanguageCode, nullable: true).Description("Culture name in ISO 3166-1 alpha-3 format");
            Field(x => x.Comment, nullable: true).Description("value of line item comment");
            Field(x => x.IsReccuring, nullable: true).Description("flag of line item is recurring");
            Field(x => x.VolumetricWeight, nullable: true).Description("Value of volumetric weight");
            Field(x => x.WeightUnit, nullable: true).Description("Value of weight unit");
            Field(x => x.Weight, nullable: true).Description("Value of shopping cart weight");
            Field(x => x.MeasureUnit, nullable: true).Description("Value of measurement unit");
            Field(x => x.Height, nullable: true).Description("Value of height");
            Field(x => x.Length, nullable: true).Description("Value of length");
            Field(x => x.Width, nullable: true).Description("Value of width");
            Field(x => x.IsReadOnly, nullable: true).Description("Is readOnly");
            Field<ObjectGraphType<MoneyType>>("listPrice", resolve: context => context.Source.ListPrice);
            Field<ObjectGraphType<MoneyType>>("paymentPlan", resolve: context => context.Source.PaymentPlan);
            Field<ObjectGraphType<MoneyType>>("listPriceWithTax", resolve: context => context.Source.ListPriceWithTax);
            Field<ObjectGraphType<MoneyType>>("salePrice", resolve: context => context.Source.SalePrice);
            Field<ObjectGraphType<MoneyType>>("salePriceWithTax", resolve: context => context.Source.SalePriceWithTax);
            Field<ObjectGraphType<MoneyType>>("placedPrice", resolve: context => context.Source.PlacedPrice);
            Field<ObjectGraphType<MoneyType>>("placedPriceWithTax", resolve: context => context.Source.PlacedPriceWithTax);
            Field<ObjectGraphType<MoneyType>>("extendedPrice", resolve: context => context.Source.ExtendedPrice);
            Field<ObjectGraphType<MoneyType>>("extendedPriceWithTax", resolve: context => context.Source.ExtendedPriceWithTax);
            Field<ObjectGraphType<MoneyType>>("discountAmount", resolve: context => context.Source.DiscountAmount);
            Field<ObjectGraphType<MoneyType>>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
            Field<ObjectGraphType<MoneyType>>("discountTotal", resolve: context => context.Source.DiscountTotal);
            Field<ObjectGraphType<MoneyType>>("discountTotalWithTax", resolve: context => context.Source.DiscountTotalWithTax);
            Field(x => x.ObjectType, nullable: true).Description("value of line item quantity");
            Field(x => x.IsValid, nullable: true).Description("value of line item quantity");
            Field<ObjectGraphType<ValidationErrorType>>("validationErrors", resolve: context => context.Source.ValidationErrors);
            Field<ObjectGraphType<MoneyType>>("taxTotal", resolve: context => context.Source.TaxTotal);
            Field(x => x.TaxPercentRate, nullable: true).Description("Value of total shipping tax amount");
            Field(x => x.TaxType, nullable: true).Description("Value of shipping tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
        }
    }
}

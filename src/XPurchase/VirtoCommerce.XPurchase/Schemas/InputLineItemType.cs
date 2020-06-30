using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputLineItemType : InputObjectGraphType<LineItem>
    {
        public InputLineItemType()
        {
            //TODO:
            //Field<ProductType>("product", resolve: context => context.Source.Product);
            //Field(x => x.InStockQuantity, nullable: true).Description("InStockQuantity");
            //Field(x => x.WarehouseLocation, nullable: true).Description("Value of line item warehouse location");
            //Field<MoneyType>("paymentPlan", resolve: context => context.Source.PaymentPlan);
            //Field(x => x.IsValid, nullable: true).Description("Value of line item quantity");
            //Field<ValidationErrorType>("validationErrors", resolve: context => context.Source.ValidationErrors);

            Field(x => x.CatalogId, nullable: true).Description("Value of catalog id");
            Field(x => x.CategoryId, nullable: true).Description("Value of category id");
            Field(x => x.CreatedDate, nullable: false).Description("Line item created date");
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
            Field<ListGraphType<InputTaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            Field<InputMoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("discountTotal", resolve: context => context.Source.DiscountTotal.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("discountTotalWithTax", resolve: context => context.Source.DiscountTotalWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("extendedPrice", resolve: context => context.Source.ExtendedPrice.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("extendedPriceWithTax", resolve: context => context.Source.ExtendedPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("listPrice", resolve: context => context.Source.ListPrice.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("listPriceWithTax", resolve: context => context.Source.ListPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("placedPrice", resolve: context => context.Source.PlacedPrice.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("placedPriceWithTax", resolve: context => context.Source.PlacedPriceWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("salePrice", resolve: context => context.Source.SalePrice.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("salePriceWithTax", resolve: context => context.Source.SalePriceWithTax.ToMoney(context.GetCart().Currency));
            Field<InputMoneyType>("taxTotal", resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));
        }
    }
}

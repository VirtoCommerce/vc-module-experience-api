using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Core.Types
{
    public class PriceType : ObjectGraphType<ProductPrice>
    {
        public PriceType()
        {
            Field<NonNullGraphType<MoneyType>>("list",
                "Price list",
                resolve: context => context.Source.ListPrice);
            Field<NonNullGraphType<MoneyType>>("listWithTax",
                "Price list with tax",
                resolve: context => context.Source.ListPriceWithTax);
            Field<NonNullGraphType<MoneyType>>("sale",
                "Sale price",
                resolve: context => context.Source.SalePrice);
            Field<NonNullGraphType<MoneyType>>("saleWithTax",
                "Sale price with tax",
                resolve: context => context.Source.SalePriceWithTax);
            Field<NonNullGraphType<MoneyType>>("actual",
                "Actual price",
                resolve: context => context.Source.ActualPrice);
            Field<NonNullGraphType<MoneyType>>("actualWithTax",
                "Actual price with tax",
                resolve: context => context.Source.ActualPriceWithTax);
            Field<NonNullGraphType<MoneyType>>("discountAmount",
                "Discount amount",
                resolve: context => context.Source.DiscountAmount);
            Field<NonNullGraphType<MoneyType>>("discountAmountWithTax",
                "Discount amount with tax",
                resolve: context => context.Source.DiscountAmountWithTax);
            Field(d => d.DiscountPercent, nullable: false);
            Field<NonNullGraphType<StringGraphType>>("currency",
                "Currency",
                resolve: context => context.Source.Currency.Code);
            Field<DateTimeGraphType>("validFrom",
                "Valid from",
                resolve: context => context.Source.StartDate, deprecationReason: "startDate");
            Field<DateTimeGraphType>("startDate",
                "Start date",
                resolve: context => context.Source.StartDate);
            Field<DateTimeGraphType>("validUntil",
                "Valid until",
                resolve: context => context.Source.EndDate, deprecationReason: "endDate");
            Field<DateTimeGraphType>("endDate",
                "End date",
                resolve: context => context.Source.EndDate);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<TierPriceType>>>>("tierPrices",
                "Tier prices",
                resolve: context => context.Source.TierPrices);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<CatalogDiscountType>>>>("discounts",
                "Discounts",
                resolve: context => context.Source.Discounts);

            Field(d => d.PricelistId, nullable: true).Description("The product price list");
            Field(d => d.MinQuantity, nullable: true).Description("The product min qty");
        }
    }
}

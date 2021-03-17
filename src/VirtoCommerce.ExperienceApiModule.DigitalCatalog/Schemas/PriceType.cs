using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class PriceType : ObjectGraphType<ProductPrice>
    {
        public PriceType()
        {
            Field<MoneyType>("list", resolve: context => context.Source.ListPrice);
            Field<MoneyType>("listWithTax", resolve: context => context.Source.ListPriceWithTax);
            Field<MoneyType>("sale", resolve: context => context.Source.SalePrice);
            Field<MoneyType>("saleWithTax", resolve: context => context.Source.SalePriceWithTax);
            Field<MoneyType>("actual", resolve: context => context.Source.ActualPrice);
            Field<MoneyType>("actualWithTax", resolve: context => context.Source.ActualPriceWithTax);
            Field<MoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount);
            Field<MoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
            Field(d => d.DiscountPercent, nullable: true);
            Field<StringGraphType>("currency", resolve: context => context.Source.Currency.Code);
            Field<DateTimeGraphType>("validFrom", resolve: context => context.Source.StartDate, deprecationReason: "startDate");
            Field<DateTimeGraphType>("startDate", resolve: context => context.Source.StartDate);
            Field<DateTimeGraphType>("validUntil", resolve: context => context.Source.EndDate, deprecationReason: "endDate");
            Field<DateTimeGraphType>("endDate", resolve: context => context.Source.EndDate);
            Field<ListGraphType<TierPriceType>>("tierPrices", resolve: context => context.Source.TierPrices);
            Field<ListGraphType<CatalogDiscountType>>("discounts", resolve: context => context.Source.Discounts);

            // TODO: remove this if not needed
            Field(d => d.PricelistId, nullable: true).Description("The product price list");
            Field(d => d.MinQuantity, nullable: true).Description("The product min qty");
        }
    }
}

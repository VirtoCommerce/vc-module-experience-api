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
            Field<MoneyType>("discountAmount", resolve: context => context.Source.DiscountAmount);
            Field<MoneyType>("discountAmountWithTax", resolve: context => context.Source.DiscountAmountWithTax);
            Field(d => d.DiscountPercent, nullable: true);
            Field<StringGraphType>("currency", resolve: context => context.Source.Currency.Code);
            Field<DateGraphType>("validFrom", resolve: context => context.Source.ValidFrom);
            Field<DateGraphType>("validUntil", resolve: context => context.Source.ValidUntil);
            Field<ListGraphType<TierPriceType>>("tierPrices", resolve: context => context.Source.TierPrices);
            Field<ListGraphType<CatalogDiscountType>>("discounts", resolve: context => context.Source.Discounts);

            // TODO: remove this if not needed
            Field(d => d.PricelistId, nullable: true).Description("The product price list");
            Field(d => d.MinQuantity, nullable: true).Description("The product min qty");
        }
    }
}

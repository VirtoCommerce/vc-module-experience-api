using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class PriceType : ObjectGraphType<Price>
    {
        public PriceType()
        {
            Field<MoneyType>("list", resolve: context => context.Source.List.ToMoney(context.GetCurrency()));
            // TODO: write resolver
            Field<MoneyType>("listWithTax", resolve: context => context.Source.List.ToMoney(context.GetCurrency()));
            Field<MoneyType>("sale", resolve: context => context.Source.List.ToMoney(context.GetCurrency()));
            // TODO: write resolver
            Field<MoneyType>("saleWithTax", resolve: context => context.Source.List.ToMoney(context.GetCurrency()));
            Field(d => d.Currency, nullable: true).Description("The product price currency");

            Field<DateGraphType>("validFrom", resolve: context => context.Source.StartDate);
            Field<DateGraphType>("validUntil", resolve: context => context.Source.EndDate);

            Field(d => d.PricelistId, nullable: true).Description("The product price list");
            Field(d => d.MinQuantity, nullable: true).Description("The product min qty");
        }
    }
}

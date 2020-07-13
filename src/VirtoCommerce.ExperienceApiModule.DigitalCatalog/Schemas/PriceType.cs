using GraphQL.Types;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class PriceType : ObjectGraphType<Price>
    {
        public PriceType()
        {
            Field(d => d.List, nullable: true).Description("The product list price");
            Field(d => d.Sale, nullable: true).Description("The product sale price");
            Field(d => d.PricelistId, nullable: true).Description("The product price list");
            Field(d => d.MinQuantity, nullable: true).Description("The product min qty");
            Field(d => d.StartDate, nullable: true).Description("The price start date");
            Field(d => d.EndDate, nullable: true).Description("The price end date");
            Field(d => d.Currency, nullable: true).Description("The product price currency");
        }
    }
}

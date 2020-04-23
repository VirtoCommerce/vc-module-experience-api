using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Extension.GraphQL.Schemas
{
    public class PriceType : ObjectGraphType<Price>
    {
        public PriceType()
        {
            Field(d => d.List, nullable: true).Description("The product list price");
            Field(d => d.Sale, nullable: true).Description("The product sale price");
            Field(d => d.PriceList, nullable: true).Description("The product price list");
            Field(d => d.Currency, nullable: true).Description("The product price currency");
        }
     
    }
}

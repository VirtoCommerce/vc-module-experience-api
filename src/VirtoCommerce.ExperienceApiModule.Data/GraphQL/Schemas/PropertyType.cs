using System.Linq;
using GraphQL.Relay.Types;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class PropertyType : ObjectGraphType<Property>
    {
        public PropertyType()
        {
            Name = "Property";
            Description = "Products are the sellable goods.";

            Field(d => d.Id).Description("The unique ID of the product.");
            Field(d => d.Name, nullable: false).Description("The name of the property.");
            Field<ListGraphType<StringGraphType>>(
                "values",
                resolve: context => context.Source.Values.Select(x=> x.ToString())
            );
        }

       
    }
}

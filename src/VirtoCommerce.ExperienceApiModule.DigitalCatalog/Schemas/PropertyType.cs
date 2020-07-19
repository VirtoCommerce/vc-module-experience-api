using System.Linq;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class PropertyType : ObjectGraphType<Property>
    {
        public PropertyType()
        {
            Name = "Property";
            Description = "Products attributes.";

            Field(x => x.Id).Description("The unique ID of the product.");
            Field(x => x.Name, nullable: false).Description("The name of the property.");
            Field<StringGraphType>(
                "valueType",
                resolve: context => context.Source.Values.Select(x => x.ValueType).FirstOrDefault()
            );
            Field<StringGraphType>(
                "value",
                resolve: context => context.Source.Values.Select(x => x.Value).FirstOrDefault()
            );
            Field<StringGraphType>(
                "valueId",
                resolve: context => context.Source.Values.Select(x => x.ValueId).FirstOrDefault()
            );
        }
    }
}

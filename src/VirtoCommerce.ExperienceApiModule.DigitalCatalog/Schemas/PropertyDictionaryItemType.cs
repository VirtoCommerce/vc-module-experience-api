using System.Linq;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class PropertyDictionaryItemType : ObjectGraphType<PropertyDictionaryItem>
    {
        public PropertyDictionaryItemType()
        {
            Name = "PropertyDictionaryItem";
            Description = "Represents property dictionary item";

            Field(x => x.Id, nullable: false).Description("The unique ID of the property dictionary item.");
            Field<StringGraphType>("value",
                resolve: context =>
                {
                    var cultureName = context.GetArgumentOrValue<string>("cultureName");
                    return string.IsNullOrEmpty(cultureName) ? context.Source.Alias : context.Source.LocalizedValues.FirstOrDefault(x => x.LanguageCode == cultureName)?.Value ?? context.Source.Alias;
                },
                description: "Value alias.");
            Field(x => x.SortOrder, nullable: false).Description("Value order.");
        }
    }
}

using System.Linq;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class PropertyType : ObjectGraphType<Property>
    {
        public PropertyType()
        {
            Name = "Property";
            Description = "Products attributes.";

            Field(x => x.Id, nullable: true).Description("The unique ID of the product.");

            Field(x => x.Name, nullable: false).Description("The name of the property.");

            Field(x => x.Hidden, nullable: false).Description("Is property hidden.");

            Field(x => x.Multivalue, nullable: false).Description("Is property has multiple values.");

            Field<StringGraphType>(
                "label",
                resolve: context =>
                {
                    var displayNames = context.Source.DisplayNames?.AsQueryable();

                    if (displayNames != null)
                    {
                        var cultureName = context.GetValue<string>("cultureName");
                        if (cultureName != null)
                        {
                            displayNames = displayNames.Where(x => x.LanguageCode == cultureName);
                        }
                    }
                    return displayNames?.Select(x => x.Name).FirstOrDefault() ?? context.Source.Name;
                });

            Field<StringGraphType>(
                "type",
                resolve: context => context.Source.Type.ToString()
            );

            Field<StringGraphType>(
                "valueType",
                resolve: context => context.Source.Values.Select(x => x.ValueType).FirstOrDefault()
            );

            Field<StringGraphType>(
                "value",
                resolve: context => context.Source.Values.Select(x => x.Value).FirstOrDefault()
            ).RootAlias("values");

            Field<StringGraphType>(
                "valueId",
                resolve: context => context.Source.Values.Select(x => x.ValueId).FirstOrDefault()
            );
        }
    }
}

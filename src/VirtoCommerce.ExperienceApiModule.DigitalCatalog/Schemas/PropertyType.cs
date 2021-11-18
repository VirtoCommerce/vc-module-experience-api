using System.Linq;
using System.Threading.Tasks;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class PropertyType : ObjectGraphType<Property>
    {
        public PropertyType(IMediator mediator, IDataLoaderContextAccessor dataLoader)
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
                    var cultureName = context.GetValue<string>("cultureName");

                    var label = cultureName != null
                        ? context.Source.DisplayNames
                            ?.FirstOrDefault(x => x.LanguageCode.EqualsInvariant(cultureName))
                            ?.Name
                        : default;

                    return string.IsNullOrWhiteSpace(label)
                        ? context.Source.Name
                        : label;
                });
            //.RootAlias("__object.properties.displayNames");

            Field<StringGraphType>(
                "type",
                resolve: context => context.Source.Type.ToString()
            );

            Field<StringGraphType>(
                "valueType",
                // since PropertyType is used both for property metadata queries and product/category/catalog queries
                // to infer "valueType" need to look in ValueType property in case of metadata query or in the first value in case
                // when the Property object was created dynamically by grouping
                resolve: context => context.Source.Values.IsNullOrEmpty()
                        ? context.Source.ValueType.ToString()
                        : context.Source.Values.Select(x => x.ValueType).FirstOrDefault().ToString()
            );

            Field<StringGraphType>(
                "value",
                resolve: context => context.Source.Values.Select(x => x.Value).FirstOrDefault()?.ToString()
            );
            //.RootAlias("__object.properties.values");

            Field<StringGraphType>(
                "valueId",
                resolve: context => context.Source.Values.Select(x => x.ValueId).FirstOrDefault()
            );

            Connection<PropertyDictionaryItemType>()
              .Name("propertyDictItems")
              .PageSize(20)
              .ResolveAsync(async context =>
              {
                  return await ResolveConnectionAsync(mediator, context);
              });

        }

        private static async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<Property> context)
        {
            var first = context.First;

            int.TryParse(context.After, out var skip);

            var query = new SearchPropertyDictionaryItemQuery
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                PropertyIds = new[] { context.Source.Id }
            };

            var response = await mediator.Send(query);

            return new PagedConnection<PropertyDictionaryItem>(response.Result.Results, query.Skip, query.Take, response.Result.TotalCount);
        }
    }
}

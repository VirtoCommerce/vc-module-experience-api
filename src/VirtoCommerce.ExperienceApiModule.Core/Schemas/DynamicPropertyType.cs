using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DynamicPropertyType : ExtendableGraphType<DynamicProperty>
    {
        public DynamicPropertyType(IMediator mediator)
        {
            Field(x => x.Id, nullable: false).Description("Id");
            Field<NonNullGraphType<StringGraphType>>("Name", resolve: context => context.Source.Name);
            Field(x => x.ObjectType, nullable: false).Description("Object type");
            Field<StringGraphType>("label",
                "Localized property name",
                resolve: context =>
            {
                var culture = context.GetValue<string>("cultureName");
                return context.Source.DisplayNames.FirstOrDefault(x => culture.IsNullOrEmpty() || x.Locale.EqualsInvariant(culture))?.Name;
            });
            Field(x => x.DisplayOrder, nullable: true).Description("The order for the dynamic property to display");
            Field<NonNullGraphType<StringGraphType>>(nameof(DynamicProperty.ValueType),
                "Value type",
                deprecationReason: "Use dynamicPropertyValueType instead",
                resolve: context => context.Source.ValueType.ToString());
            Field<NonNullGraphType<DynamicPropertyValueTypeEnum>>("dynamicPropertyValueType",
                "Value type",
                resolve: context => context.Source.ValueType);
            Field<NonNullGraphType<BooleanGraphType>>("isArray", resolve: context => context.Source.IsArray, description: "Is dynamic property value an array");
            Field<NonNullGraphType<BooleanGraphType>>("isDictionary", resolve: context => context.Source.IsDictionary, description: "Is dynamic property value a dictionary");
            Field<NonNullGraphType<BooleanGraphType>>("isMultilingual", resolve: context => context.Source.IsMultilingual, description: "Is dynamic property value multilingual");
            Field<NonNullGraphType<BooleanGraphType>>("isRequired", resolve: context => context.Source.IsRequired, description: "Is dynamic property value required");

            Connection<DictionaryItemType>()
              .Name("dictionaryItems")
              .Argument<StringGraphType>("filter", "")
              .Argument<StringGraphType>("cultureName", "")
              .Argument<StringGraphType>("sort", "")
              .PageSize(20)
              .ResolveAsync(async context =>
              {
                  return await ResolveConnectionAsync(mediator, context);
              });
        }

        private static async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<DynamicProperty> context)
        {
            int.TryParse(context.After, out var skip);

            var query = context.GetDynamicPropertiesQuery<SearchDynamicPropertyDictionaryItemQuery>();
            query.PropertyId = context.Source.Id;
            query.Skip = skip;
            query.Take = context.First ?? context.PageSize ?? 10;
            query.Sort = context.GetArgument<string>("sort");
            query.Filter = context.GetArgument<string>("filter");

            context.CopyArgumentsToUserContext();

            var response = await mediator.Send(query);

            return new PagedConnection<DynamicPropertyDictionaryItem>(response.Results, query.Skip, query.Take, response.TotalCount);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DynamicPropertyType : ExtendableGraphType<DynamicPropertyEntity>
    {
        public DynamicPropertyType(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            Field(x => x.Id).Description("Id");
            Field(x => x.Name).Description("Name");
            Field(x => x.ObjectType).Description("ObjectType");
            Field(x => x.ValueType).Description("ValueType");
            Field(x => x.DisplayOrder, nullable: true).Description("Is dynamic property is required");
            // PT-737: Add label
            Field<BooleanGraphType>("isArray", resolve: context => context.Source.IsArray, description: "Is dynamic property is an array");
            Field<BooleanGraphType>("isDictionary", resolve: context => context.Source.IsDictionary, description: "Is dynamic property is an dictionary");
            Field<BooleanGraphType>("isMultilingual", resolve: context => context.Source.IsMultilingual, description: "Is dynamic property is multilingual");
            Field<BooleanGraphType>("isRequired", resolve: context => context.Source.IsRequired, description: "Is dynamic property is required");

            Connection<DictionaryItemType>()
              .Name("dictionaryItems")
              .Argument<StringGraphType>("filter", "")
              .Argument<StringGraphType>("cultureName", "")
              .Argument<StringGraphType>("sort", "")
              .Unidirectional()
              .PageSize(20)
              .ResolveAsync(async context =>
              {
                  return await ResolveConnectionAsync(mediator, context);
              });
        }

        private static async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<DynamicPropertyEntity> context)
        {
            var first = context.First;

            int.TryParse(context.After, out var skip);

            return new PagedConnection<DynamicPropertyDictionaryItemEntity>(Enumerable.Empty<DynamicPropertyDictionaryItemEntity>(), skip, Convert.ToInt32(context.After ?? 0.ToString()), 0);
        }
    }
}

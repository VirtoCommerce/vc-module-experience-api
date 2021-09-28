using System.Linq;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DynamicPropertyValueType : ExtendableGraphType<DynamicPropertyObjectValue>
    {
        private readonly IDynamicPropertyDictionaryItemsService _dynamicPropertyDictionaryItemsService;

        public DynamicPropertyValueType(IMediator mediator, IDynamicPropertyDictionaryItemsService dynamicPropertyDictionaryItemsService)
        {
            _dynamicPropertyDictionaryItemsService = dynamicPropertyDictionaryItemsService;

            Field<StringGraphType>("name", "Property Name", resolve: context => context.Source.PropertyName);
            Field<StringGraphType>(nameof(DynamicPropertyObjectValue.ValueType), "Value Type", resolve: context => context.Source.ValueType.ToString());
            Field<StringGraphType>(nameof(DynamicPropertyObjectValue.Value), "Property Value", resolve: context => context.Source.Value?.ToString());

            FieldAsync<DictionaryItemType>("dictionaryItem", "Associated dictionary item", resolve: async context =>
            {
                var id = context.Source.ValueId;
                if (id.IsNullOrEmpty()) return null;

                var items = await _dynamicPropertyDictionaryItemsService.GetDynamicPropertyDictionaryItemsAsync(new[] { id });

                return items.FirstOrDefault();
            });

            FieldAsync<DynamicPropertyType>("dynamicProperty", "Associated dynamic property", resolve: async context =>
            {
                var id = context.Source.PropertyId;
                if (id.IsNullOrEmpty()) return null;

                var query = context.GetDynamicPropertiesQuery<GetDynamicPropertyQuery>();
                query.IdOrName = id;

                var response = await mediator.Send(query);

                return response.DynamicProperty;
            });
        }
    }
}

using System.Linq;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes;
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

            Field<StringGraphType>("name",
                "Property name",
                resolve: context => context.Source.PropertyName);
            Field<NonNullGraphType<DynamicPropertyValueTypeType>>(nameof(DynamicPropertyObjectValue.ValueType),
                "Value type",
                resolve: context => context.Source.ValueType);
            Field<DynamicPropertyValueGraphType>(nameof(DynamicPropertyObjectValue.Value),
                "Property value",
                resolve: context => context.Source.Value);

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

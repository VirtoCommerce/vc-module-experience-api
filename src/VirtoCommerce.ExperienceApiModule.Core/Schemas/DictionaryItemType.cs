using System.Linq;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DictionaryItemType : ExtendableGraphType<DynamicPropertyDictionaryItem>
    {
        public DictionaryItemType()
        {
            Field(x => x.Id).Description("Id");
            Field(x => x.Name).Description("Name");
            Field<StringGraphType>("label",
                "Localized dictionary item value",
                resolve: context =>
            {
                var culture = context.GetValue<string>("cultureName");
                return context.Source.DisplayNames.FirstOrDefault(x => culture.IsNullOrEmpty() || x.Locale.EqualsInvariant(culture))?.Name;
            });
        }
    }
}

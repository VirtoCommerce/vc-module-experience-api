using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas;

public class ModuleSettingType : ObjectGraphType<ModuleSetting>
{
    public ModuleSettingType()
    {
        Field(x => x.Name, nullable: false);
        Field<ModuleSettingValueGraphType>("value", resolve: x => x.Source.Value);
    }
}

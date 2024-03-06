using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas;

public class ModuleSettingType : ObjectGraphType<ModuleSetting>
{
    public ModuleSettingType()
    {
        Field(x => x.Name, nullable: false);
        Field(x => x.Value, nullable: true);
    }
}

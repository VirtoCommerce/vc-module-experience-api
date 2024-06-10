using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas;

public class ModuleSettingsType : ObjectGraphType<ModuleSettings>
{
    public ModuleSettingsType()
    {
        Field(x => x.ModuleId, nullable: false);
        Field<NonNullGraphType<ListGraphType<NonNullGraphType<ModuleSettingType>>>>("settings");
    }
}

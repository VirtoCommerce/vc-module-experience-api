namespace VirtoCommerce.ExperienceApiModule.Core.Queries;

public class ModuleSetting
{
    public string Name { get; set; }
    public object Value { get; set; }
}

public class ModuleSettings
{
    public string ModuleId { get; set; }

    public ModuleSetting[] Settings { get; set; }
}

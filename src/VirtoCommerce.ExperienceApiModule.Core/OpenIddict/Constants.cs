namespace VirtoCommerce.ExperienceApiModule.Core.OpenIddict;

public static class GrantTypes
{
    public const string SwitchOrganization = "switch_organization";
}

public static class Parameters
{
    public const string StoreId = "storeId";
    public const string OrganizationId = "organization_id";
}

public static class Claims
{
    public const string OrganizationId = "organization_id";
}

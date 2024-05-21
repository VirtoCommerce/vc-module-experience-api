using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas;

public class StoreSettingsType : ExtendableGraphType<StoreSettings>
{
    public StoreSettingsType()
    {
        Field(x => x.QuotesEnabled).Description("Store ID");
        Field(x => x.SubscriptionEnabled).Description("Store ID");
        Field(x => x.TaxCalculationEnabled).Description("Tax calculation enabled");
        Field(x => x.AnonymousUsersAllowed).Description("Allow anonymous users to visit the store ");
        Field(x => x.IsSpa).Description("SPA");
        Field(x => x.EmailVerificationEnabled).Description("Email address verification enabled");
        Field(x => x.EmailVerificationRequired).Description("Email address verification required");
        Field(x => x.CreateAnonymousOrderEnabled).Description("Allow anonymous users to create orders (XAPI)");
        Field(x => x.SeoLinkType).Description("SEO links");
        Field(x => x.DefaultSelectedForCheckout).Description("Default \"Selected for checkout\" state for new line items and gifts");
        Field<PasswordOptionsType>("passwordRequirements", "Password requirements");
        Field<NonNullGraphType<ListGraphType<NonNullGraphType<ModuleSettingsType>>>>("modules");
    }
}

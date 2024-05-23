using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class StoreSettings
    {
        public bool QuotesEnabled { get; set; }

        public bool SubscriptionEnabled { get; set; }

        public bool TaxCalculationEnabled { get; set; }

        public bool AnonymousUsersAllowed { get; set; }

        public bool IsSpa { get; set; }

        public bool EmailVerificationEnabled { get; set; }

        public bool EmailVerificationRequired { get; set; }

        public bool CreateAnonymousOrderEnabled { get; set; }

        public string SeoLinkType { get; set; }

        public string EnvironmentName { get; set; }

        public PasswordOptions PasswordRequirements { get; set; }

        public ModuleSettings[] Modules { get; set; }
    }
}

using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class InviteUserCommand: ICommand<IdentityResultResponse>
    {
        public string StoreId { get; set; }

        public string OrganizationId { get; set; }

        public string UrlSuffix { get; set; }

        public string[] Emails { get; set; }

        public string Message { get; set; }
    }
}

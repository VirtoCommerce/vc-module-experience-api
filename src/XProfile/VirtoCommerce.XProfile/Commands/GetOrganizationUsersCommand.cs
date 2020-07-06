using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class GetOrganizationUsersCommand : IRequest<ProfileSearchResult>
    {
        public string OrganizationId { get; set; }

        public string UserId { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
        public string Sort { get; set; }
    }
}

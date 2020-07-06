using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class OrganizationCommand : Organization, IRequest<Organization>
    {
        public OrganizationCommand()
        {
            MemberType = nameof(Organization);
        }
    }
}

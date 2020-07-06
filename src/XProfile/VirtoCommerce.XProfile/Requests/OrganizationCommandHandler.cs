using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class OrganizationCommandHandler : IRequestHandler<OrganizationCommand, Organization>
    {
        private readonly IMemberServiceX _memberService;

        public OrganizationCommandHandler(IMemberServiceX memberService)
        {
            _memberService = memberService;
        }

        public async Task<Organization> Handle(OrganizationCommand request, CancellationToken cancellationToken)
        {
            return await _memberService.UpdateOrganizationAsync(request);
        }
    }
}

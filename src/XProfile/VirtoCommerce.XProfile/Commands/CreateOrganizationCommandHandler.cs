using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Organization>
    {
        private readonly IMemberServiceX _memberServiceX;
        public CreateOrganizationCommandHandler(IMemberServiceX memberServiceX)
        {
            _memberServiceX = memberServiceX;
        }

        public Task<Organization> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            return _memberServiceX.CreateOrganizationAsync(request.Organization);
        }
    }
}

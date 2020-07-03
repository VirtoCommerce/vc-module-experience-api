using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, IdentityResult>
    {
        private readonly IMemberServiceX _memberService;

        public LockUserCommandHandler(IMemberServiceX memberService)
        {
            _memberService = memberService;
        }

        public async Task<IdentityResult> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            return await _memberService.LockUserAsync(request.UserId);
        }
    }
}

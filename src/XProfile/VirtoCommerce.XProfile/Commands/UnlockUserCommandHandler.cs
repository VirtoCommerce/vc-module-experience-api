using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, IdentityResult>
    {
        private readonly IMemberServiceX _memberService;

        public UnlockUserCommandHandler(IMemberServiceX memberService)
        {
            _memberService = memberService;
        }

        public async Task<IdentityResult> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            return await _memberService.UnlockUserAsync(request.UserId);
        }
    }
}

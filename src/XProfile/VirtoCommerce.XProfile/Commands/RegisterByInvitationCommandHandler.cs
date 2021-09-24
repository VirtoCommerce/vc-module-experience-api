using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class RegisterByInvitationCommandHandler : IRequestHandler<RegisterByInvitationCommand, IdentityResultResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly IMemberService _memberService;

        public RegisterByInvitationCommandHandler(Func<UserManager<ApplicationUser>> userManager, IMemberService memberService)
        {
            _userManagerFactory = userManager;
            _memberService = memberService;
        }

        public virtual async Task<IdentityResultResponse> Handle(RegisterByInvitationCommand request, CancellationToken cancellationToken)
        {
            using var userManager = _userManagerFactory();

            var result = new IdentityResultResponse();
            var identityResult = default(IdentityResult);

            var user = await userManager.FindByIdAsync(request.UserId);

            if (user != null)
            {
                identityResult = await userManager.SetUserNameAsync(user, request.Username);
                if (identityResult.Succeeded)
                {
                    identityResult = await userManager.AddPasswordAsync(user, request.Password);

                    if (identityResult.Succeeded)
                    {
                        var contact = await _memberService.GetByIdAsync(user.MemberId) as Contact;
                        if (contact != null)
                        {
                            contact.FirstName = request.FirstName;
                            contact.LastName = request.LastName;
                            contact.FullName = $"{request.FirstName} {request.LastName}";

                            await _memberService.SaveChangesAsync(new Member[] { contact });
                        }
                    }
                }
            }

            result.Errors = identityResult?.Errors.Select(x => x.MapToIdentityErrorInfo()).ToList();
            result.Succeeded = identityResult?.Succeeded ?? false;

            return result;
        }
    }
}

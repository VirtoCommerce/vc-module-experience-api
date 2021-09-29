using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
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
        private readonly IWebHostEnvironment _environment;
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly IMemberService _memberService;

        public RegisterByInvitationCommandHandler(
            IWebHostEnvironment environment,
            Func<UserManager<ApplicationUser>> userManager, IMemberService memberService)
        {
            _environment = environment;
            _userManagerFactory = userManager;
            _memberService = memberService;
        }

        public virtual async Task<IdentityResultResponse> Handle(RegisterByInvitationCommand request, CancellationToken cancellationToken)
        {
            using var userManager = _userManagerFactory();

            var result = new IdentityResultResponse();
            IdentityResult identityResult;

            var user = await userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                var errors = _environment.IsDevelopment() ? new [] { new IdentityError { Code = "UserNotFound", Description = "User not found" } } : null;
                identityResult = IdentityResult.Failed(errors);
            }
            else
            {
                identityResult = await userManager.ResetPasswordAsync(user, Uri.UnescapeDataString(request.Token), request.Password);
                if (identityResult.Succeeded)
                {
                    identityResult = await userManager.SetUserNameAsync(user, request.Username);
                    if (identityResult.Succeeded)
                    {
                        var contact = await _memberService.GetByIdAsync(user.MemberId) as Contact;
                        if (contact == null)
                        {
                            var errors = _environment.IsDevelopment() ? new [] { new IdentityError { Code = "ContactNotFound", Description = "Contact not found" } } : null;
                            identityResult = IdentityResult.Failed(errors);
                        }
                        else
                        {
                            contact.FirstName = request.FirstName;
                            contact.LastName = request.LastName;
                            contact.FullName = $"{request.FirstName} {request.LastName}";
                            contact.Phones = new List<string> { request.Phone };

                            await _memberService.SaveChangesAsync(new Member[] { contact });
                        }
                    }
                }
            }

            result.Errors = identityResult.Errors?.Select(x => x.MapToIdentityErrorInfo()).ToList();
            result.Succeeded = identityResult.Succeeded;

            return result;
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Extensions;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class ResetPasswordByTokenCommandHandler : IRequestHandler<ResetPasswordByTokenCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthorizationOptions _securityOptions;

        public ResetPasswordByTokenCommandHandler(
            Func<UserManager<ApplicationUser>> userManagerFactory,
            IOptions<AuthorizationOptions> securityOptions)
        {
            _userManager = userManagerFactory();
            _securityOptions = securityOptions.Value;
        }

        public virtual async Task<IdentityResult> Handle(ResetPasswordByTokenCommand request, CancellationToken cancellationToken)
        {
            IdentityResult result;
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                result = IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }
            else if (!IsUserEditable(user.UserName))
            {
                result = IdentityResult.Failed(new IdentityError { Description = "It is forbidden to edit this user." });
            }
            else
            {
                result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

                if (result.Succeeded && user.PasswordExpired)
                {
                    user.PasswordExpired = false;
                    await _userManager.UpdateAsync(user);
                }
            }

            return result;
        }

        protected virtual bool IsUserEditable(string userName)
        {
            var result = _securityOptions.NonEditableUsers?.FirstOrDefault(x => x.EqualsInvariant(userName)) == null;

            return result;
        }
    }
}

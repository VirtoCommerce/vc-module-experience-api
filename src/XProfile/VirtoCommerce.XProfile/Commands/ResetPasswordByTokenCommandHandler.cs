using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class ResetPasswordByTokenCommandHandler : IRequestHandler<ResetPasswordByTokenCommand, IdentityResultResponse>
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

        public virtual async Task<IdentityResultResponse> Handle(ResetPasswordByTokenCommand request, CancellationToken cancellationToken)
        {
            var result = new IdentityResultResponse();
            IdentityResult identityResult;
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                identityResult = IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }
            else if (!IsUserEditable(user.UserName))
            {
                identityResult = IdentityResult.Failed(new IdentityError { Description = "It is forbidden to edit this user." });
            }
            else
            {
                identityResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

                if (identityResult.Succeeded && user.PasswordExpired)
                {
                    user.PasswordExpired = false;
                    await _userManager.UpdateAsync(user);
                }
            }

            result.Errors = identityResult?.Errors.Select(x => x.MapToIdentityErrorInfo()).ToArray();
            result.Succeeded = identityResult?.Succeeded ?? false;

            return result;
        }

        protected virtual bool IsUserEditable(string userName)
        {
            var result = _securityOptions.NonEditableUsers?.FirstOrDefault(x => x.EqualsInvariant(userName)) == null;

            return result;
        }
    }
}

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
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly AuthorizationOptions _securityOptions;

        public ResetPasswordByTokenCommandHandler(
            Func<UserManager<ApplicationUser>> userManagerFactory,
            IOptions<AuthorizationOptions> securityOptions)
        {
            _userManagerFactory = userManagerFactory;
            _securityOptions = securityOptions.Value;
        }

        public virtual async Task<IdentityResultResponse> Handle(ResetPasswordByTokenCommand request, CancellationToken cancellationToken)
        {
            var result = new IdentityResultResponse();
            IdentityResult identityResult;

            using var userManager = _userManagerFactory();

            var user = await userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                identityResult = IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = "User not found" });
            }
            else if (!IsUserEditable(user.UserName))
            {
                identityResult = IdentityResult.Failed(new IdentityError { Code = "UserIsNotEditable", Description = "It is forbidden to edit this user." });
            }
            else
            {
                identityResult = await userManager.ResetPasswordAsync(user, Uri.UnescapeDataString(request.Token), request.NewPassword);

                if (identityResult.Succeeded && user.PasswordExpired)
                {
                    user.PasswordExpired = false;
                    await userManager.UpdateAsync(user);
                }
            }

            result.Errors = identityResult?.Errors.Select(x => x.MapToIdentityErrorInfo()).ToList();
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

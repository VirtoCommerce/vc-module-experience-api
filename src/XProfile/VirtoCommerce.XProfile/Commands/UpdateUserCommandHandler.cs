using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateUserCommandHandler : UserCommandHandlerBase, IRequestHandler<UpdateUserCommand, IdentityResult>
    {
        public UpdateUserCommandHandler(Func<UserManager<ApplicationUser>> userManager, IOptions<AuthorizationOptions> securityOptions)
            : base(userManager, securityOptions)
        {
        }

        public async Task<IdentityResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (!IsUserEditable(request.UserName))
            {
                return IdentityResult.Failed(new IdentityError { Description = "It is forbidden to edit this user." });
            }

            using (var userManager = _userManagerFactory())
                return await userManager.UpdateAsync(request);
        }
    }
}

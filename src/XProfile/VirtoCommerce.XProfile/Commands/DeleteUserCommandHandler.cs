using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{

    public class DeleteUserCommandHandler : UserCommandHandlerBase, IRequestHandler<DeleteUserCommand, IdentityResult>
    {
        public DeleteUserCommandHandler(Func<UserManager<ApplicationUser>> userManager, IOptions<AuthorizationOptions> securityOptions)
            : base(userManager, securityOptions)
        {
        }

        public async Task<IdentityResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserNames.Any(x => !IsUserEditable(x)))
            {
                return IdentityResult.Failed(new IdentityError() { Description = "It is forbidden to edit these users." });
            }

            using (var userManager = _userManagerFactory())
            foreach (var userName in request.UserNames)
            {
                var user = await userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    var result = await userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
            }

            return IdentityResult.Success;
        }
    }
}

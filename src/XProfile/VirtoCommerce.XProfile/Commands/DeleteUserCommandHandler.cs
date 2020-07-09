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
        public DeleteUserCommandHandler(IServiceProvider services, IOptions<AuthorizationOptions> securityOptions)
            : base(services, securityOptions)
        {
        }

        public async Task<IdentityResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserNames.Any(x => !IsUserEditable(x)))
            {
                return IdentityResult.Failed(new IdentityError() { Description = "It is forbidden to edit these users." });
            }

            using var scope = _services.CreateScope();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var userName in request.UserNames)
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
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

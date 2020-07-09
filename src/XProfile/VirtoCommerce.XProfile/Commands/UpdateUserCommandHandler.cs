using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateUserCommandHandler : UserCommandHandlerBase, IRequestHandler<UpdateUserCommand, IdentityResult>
    {
        public UpdateUserCommandHandler(IServiceProvider services, IOptions<AuthorizationOptions> securityOptions)
            : base(services, securityOptions)
        {
        }

        public async Task<IdentityResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (!IsUserEditable(request.UserName))
            {
                return IdentityResult.Failed(new IdentityError { Description = "It is forbidden to edit this user." });
            }

            using var scope = _services.CreateScope();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return await _userManager.UpdateAsync(request);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IdentityResult>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public CreateUserCommandHandler(Func<UserManager<ApplicationUser>> userManager)
        {
            _userManagerFactory = userManager;
        }

        public async Task<IdentityResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            using (var userManager = _userManagerFactory())
            {
                if (request.Password.IsNullOrEmpty())
                {
                    return await userManager.CreateAsync(request);
                }
                else
                {
                    return await userManager.CreateAsync(request, request.Password);
                }
            }
        }
    }
}

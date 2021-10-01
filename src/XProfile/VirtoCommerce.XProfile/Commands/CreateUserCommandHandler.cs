using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IdentityResult>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly IStoreNotificationSender _storeNotificationSender;

        public CreateUserCommandHandler(Func<UserManager<ApplicationUser>> userManager, IStoreNotificationSender storeNotificationSender)
        {
            _userManagerFactory = userManager;
            _storeNotificationSender = storeNotificationSender;
        }

        public virtual async Task<IdentityResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            using (var userManager = _userManagerFactory())
            {
                var result = default(IdentityResult);

                if (request.ApplicationUser.Password.IsNullOrEmpty())
                {
                    result = await userManager.CreateAsync(request.ApplicationUser);
                }
                else
                {
                    result = await userManager.CreateAsync(request.ApplicationUser, request.ApplicationUser.Password);
                }

                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(request.ApplicationUser.UserName);

                    await _storeNotificationSender.SendUserEmailVerificationAsync(user);
                }

                return result;
            }
        }
    }
}

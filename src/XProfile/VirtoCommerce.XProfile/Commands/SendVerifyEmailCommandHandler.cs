using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;


namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class SendVerifyEmailCommandHandler : IRequestHandler<SendVerifyEmailCommand, bool>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly IStoreNotificationSender _storeNotificationSender;

        public SendVerifyEmailCommandHandler(Func<UserManager<ApplicationUser>> userManagerFactory, IStoreNotificationSender storeNotificationSender)
        {
            _userManagerFactory = userManagerFactory;
            _storeNotificationSender = storeNotificationSender;
        }

        public async Task<bool> Handle(SendVerifyEmailCommand request, CancellationToken cancellationToken)
        {
            using (var userManager = _userManagerFactory())
            {
                var user = await userManager.FindByEmailAsync(request.Email);

                if (user == null)
                    return true;

                await _storeNotificationSender.SendUserEmailVerificationAsync(user);

                return true;
            }
        }
    }
}

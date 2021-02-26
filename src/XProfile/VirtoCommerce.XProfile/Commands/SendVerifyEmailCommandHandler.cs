using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;


namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class SendVerifyEmailCommandHandler : IRequestHandler<SendVerifyEmailCommand, bool>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly IStoreNotificationSender _storeNotificationSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SendVerifyEmailCommandHandler(Func<UserManager<ApplicationUser>> userManagerFactory, IStoreNotificationSender storeNotificationSender, IHttpContextAccessor httpContextAccessor)
        {
            _userManagerFactory = userManagerFactory;
            _storeNotificationSender = storeNotificationSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(SendVerifyEmailCommand request, CancellationToken cancellationToken)
        {
            using (var userManager = _userManagerFactory())
            {
                var user = await GetUserAsync(request.Email, userManager);

                if (user == null)
                    return true;

                await SendUserEmailVerificationAsync(user);

                return true;
            }
        }

        private async Task<ApplicationUser> GetUserAsync(string userEmail, UserManager<ApplicationUser> userManager)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null &&
                context.User != null &&
                context.User.Identity != null &&
                context.User.Identity.IsAuthenticated)
            {
                return await userManager.FindByNameAsync(context.User.Identity.Name);
            }
            else
            {
                return await userManager.FindByEmailAsync(userEmail);
            }
        }

        private async Task SendUserEmailVerificationAsync(ApplicationUser user)
        {
            try
            {
                await _storeNotificationSender.SendUserEmailVerificationAsync(user);
            }
            catch (Exception)
            {
                // suppress all exceptions 
            }
        }
    }
}

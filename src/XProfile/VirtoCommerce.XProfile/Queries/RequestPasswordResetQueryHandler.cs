using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class RequestPasswordResetQueryHandler : IQueryHandler<RequestPasswordResetQuery, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationSender _notificationSender;

        public RequestPasswordResetQueryHandler(
            Func<SignInManager<ApplicationUser>> signInManagerFactory,
            INotificationSearchService notificationSearchService,
            INotificationSender notificationSender
            )
        {
            _userManager = signInManagerFactory().UserManager;
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
        }

        public async Task<bool> Handle(RequestPasswordResetQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.LoginOrEmail)
                       ?? await _userManager.FindByEmailAsync(request.LoginOrEmail);

            if (!string.IsNullOrEmpty(user?.Email))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrlWithoutLastSlash = request.CallbackUrl.EndsWith("/")
                    ? request.CallbackUrl[..^1]
                    : request.CallbackUrl;

                var notification = (ResetPasswordEmailNotification)await _notificationSearchService.GetNotificationAsync(nameof(ResetPasswordEmailNotification));
                notification.Url = $"{callbackUrlWithoutLastSlash}/{user.Id}/{token}";
                notification.To = user.Email;
                notification.From = "noreply@gmail.com";

                await _notificationSender.ScheduleSendNotificationAsync(notification);
            }

            return true;
        }
    }
}

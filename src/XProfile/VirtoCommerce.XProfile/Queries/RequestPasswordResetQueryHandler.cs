using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class RequestPasswordResetQueryHandler : IQueryHandler<RequestPasswordResetQuery, bool>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationSender _notificationSender;
        private readonly IStoreService _storeService;

        public RequestPasswordResetQueryHandler(
            Func<UserManager<ApplicationUser>> userManagerFactory,
            INotificationSearchService notificationSearchService,
            INotificationSender notificationSender,
            IStoreService storeService
            )
        {
            _userManagerFactory = userManagerFactory;
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
            _storeService = storeService;
        }

        public virtual async Task<bool> Handle(RequestPasswordResetQuery request, CancellationToken cancellationToken)
        {
            using var userManager = _userManagerFactory();

            var user = await userManager.FindByNameAsync(request.LoginOrEmail)
                       ?? await userManager.FindByEmailAsync(request.LoginOrEmail);

            if (!string.IsNullOrEmpty(user?.Email) && !string.IsNullOrEmpty(user.StoreId))
            {
                var store = await _storeService.GetByIdAsync(user.StoreId);

                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var notification = await _notificationSearchService.GetNotificationAsync<ResetPasswordEmailNotification>();
                notification.Url = $"{store.Url.TrimLastSlash()}{request.UrlSuffix.NormalizeUrlSuffix()}/{user.Id}/{token}";
                notification.To = user.Email;
                notification.From = store.Email;

                await _notificationSender.ScheduleSendNotificationAsync(notification);
            }

            return true;
        }
    }
}

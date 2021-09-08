using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class RequestPasswordResetQueryHandler : IQueryHandler<RequestPasswordResetQuery, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationSender _notificationSender;
        private readonly IStoreService _storeService;

        public RequestPasswordResetQueryHandler(
            Func<SignInManager<ApplicationUser>> signInManagerFactory,
            INotificationSearchService notificationSearchService,
            INotificationSender notificationSender,
            IStoreService storeService
            )
        {
            _userManager = signInManagerFactory().UserManager;
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
            _storeService = storeService;
        }

        public virtual async Task<bool> Handle(RequestPasswordResetQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.LoginOrEmail)
                       ?? await _userManager.FindByEmailAsync(request.LoginOrEmail);

            if (!string.IsNullOrEmpty(user?.Email) && !string.IsNullOrEmpty(user.StoreId))
            {
                var store = await _storeService.GetByIdAsync(user.StoreId);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrlWithoutLastSlash = store.Url.EndsWith("/")
                    ? store.Url[..^1]
                    : store.Url;
                var urlSuffix = NormalizeUrlSuffix(request.UrlSuffix);

                var notification = await _notificationSearchService.GetNotificationAsync<ResetPasswordEmailNotification>();
                notification.Url = $"{callbackUrlWithoutLastSlash}{urlSuffix}/{user.Id}/{token}";
                notification.To = user.Email;
                notification.From = store.Email;

                await _notificationSender.ScheduleSendNotificationAsync(notification);
            }

            return true;
        }

        /// <summary>
        /// Normalize values like "/reset/" and "reset"
        /// to "/reset"
        /// </summary>
        /// <param name="urlSuffix"></param>
        /// <returns></returns>
        protected virtual string NormalizeUrlSuffix(string urlSuffix)
        {
            var result = new StringBuilder(urlSuffix);

            if (!urlSuffix.StartsWith("/"))
            {
                result.Insert(0, "/");
            }

            if (urlSuffix.EndsWith("/"))
            {
                result.Remove(result.Length - 1, 1);
            }

            return result.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class InviteUserCommandHandler : IRequestHandler<InviteUserCommand, IdentityResultResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly IMemberService _memberService;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationSender _notificationSender;
        private readonly IStoreService _storeService;

        public InviteUserCommandHandler(
            Func<UserManager<ApplicationUser>> userManager, IMemberService memberService,
            INotificationSearchService notificationSearchService, INotificationSender notificationSender,
            IStoreService storeService)
        {
            _userManagerFactory = userManager;
            _memberService = memberService;
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
            _storeService = storeService;
        }

        public virtual async Task<IdentityResultResponse> Handle(InviteUserCommand request, CancellationToken cancellationToken)
        {
            using var userManager = _userManagerFactory();

            var result = new IdentityResultResponse();
            var identityResult = default(IdentityResult);

            foreach (var email in request.Emails)
            {
                var contact = new Contact { FirstName = string.Empty, LastName = string.Empty, FullName = string.Empty, Organizations = new List<string> { request.OrganizationId }};
                await _memberService.SaveChangesAsync(new Member[] { contact });

                var user = new ApplicationUser { UserName = email, Email = email, MemberId = contact.Id, StoreId = request.StoreId };
                identityResult = await userManager.CreateAsync(user);

                if (identityResult.Succeeded)
                {
                    var store = await _storeService.GetByIdAsync(user.StoreId);
                    if (store == null)
                    {
                        identityResult = IdentityResult.Failed(new IdentityError { Code = "StoreNotFound", Description = "Store not found" });
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(store.Url) || string.IsNullOrEmpty(store.Email))
                        {
                            identityResult = IdentityResult.Failed(new IdentityError { Code = "StoreNotConfigured", Description = "Store has invalid URL or email" });
                        }
                        else
                        {
                            user = await userManager.FindByEmailAsync(email);
                            var token = await userManager.GeneratePasswordResetTokenAsync(user);

                            var notification = await _notificationSearchService.GetNotificationAsync<RegistrationInvitationEmailNotification>();
                            notification.InviteUrl = $"{store.Url.TrimLastSlash()}{request.UrlSuffix.NormalizeUrlSuffix()}?userId={user.Id}&token={Uri.EscapeDataString(token)}";
                            notification.Message = request.Message;
                            notification.To = user.Email;
                            notification.From = store.Email;

                            await _notificationSender.ScheduleSendNotificationAsync(notification);
                        }
                    }
                }
            }

            result.Errors = identityResult?.Errors.Select(x => x.MapToIdentityErrorInfo()).ToList();
            result.Succeeded = identityResult?.Succeeded ?? false;

            return result;
        }
    }
}

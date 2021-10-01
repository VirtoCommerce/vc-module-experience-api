using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class InviteUserCommandHandler : IRequestHandler<InviteUserCommand, IdentityResultResponse>
    {
        private readonly IWebHostEnvironment _environment;
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly IMemberService _memberService;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationSender _notificationSender;
        private readonly IStoreService _storeService;

        public InviteUserCommandHandler(
            IWebHostEnvironment environment,
            Func<UserManager<ApplicationUser>> userManager, IMemberService memberService,
            INotificationSearchService notificationSearchService, INotificationSender notificationSender,
            IStoreService storeService)
        {
            _environment = environment;
            _userManagerFactory = userManager;
            _memberService = memberService;
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
            _storeService = storeService;
        }

        public virtual async Task<IdentityResultResponse> Handle(InviteUserCommand request, CancellationToken cancellationToken)
        {
            using var userManager = _userManagerFactory();

            var result = new IdentityResultResponse
            {
                Errors = new List<IdentityErrorInfo>(),
                Succeeded = true,
            };

            foreach (var email in request.Emails)
            {
                var contact = new Contact { FirstName = string.Empty, LastName = string.Empty, FullName = string.Empty, Organizations = new List<string> { request.OrganizationId }};
                await _memberService.SaveChangesAsync(new Member[] { contact });

                var user = new ApplicationUser { UserName = email, Email = email, MemberId = contact.Id, StoreId = request.StoreId, UserType = UserType.Customer.ToString() };
                var identityResult = await userManager.CreateAsync(user);

                if (identityResult.Succeeded)
                {
                    var store = await _storeService.GetByIdAsync(user.StoreId);
                    if (store == null)
                    {
                        var errors = _environment.IsDevelopment() ? new [] { new IdentityError { Code = "StoreNotFound", Description = "Store not found" } } : null;
                        identityResult = IdentityResult.Failed(errors);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(store.Url) || string.IsNullOrEmpty(store.Email))
                        {
                            var errors = _environment.IsDevelopment() ? new [] { new IdentityError { Code = "StoreNotConfigured", Description = "Store has invalid URL or email" } } : null;
                            identityResult = IdentityResult.Failed(errors);
                        }
                        else
                        {
                            await SendNotificationAsync(request, store, email);
                        }
                    }
                }
                
                result.Errors.AddRange(identityResult.Errors.Select(x => x.MapToIdentityErrorInfo()));
                result.Succeeded &= identityResult.Succeeded;

                if (!result.Succeeded)
                {
                    await _memberService.DeleteAsync(new[] { contact.Id });

                    if (user.Id != null)
                    {
                        await userManager.DeleteAsync(user);
                    }
                }
            }

            return result;
        }

        protected virtual async Task SendNotificationAsync(InviteUserCommand request, Store store, string email)
        {
            using var userManager = _userManagerFactory();

            var user = await userManager.FindByEmailAsync(email);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var notification = await _notificationSearchService.GetNotificationAsync<RegistrationInvitationEmailNotification>();
            notification.InviteUrl = $"{store.Url.TrimLastSlash()}{request.UrlSuffix.NormalizeUrlSuffix()}?userId={user.Id}&email={user.Email}&token={Uri.EscapeDataString(token)}";
            notification.Message = request.Message;
            notification.To = user.Email;
            notification.From = store.Email;

            await _notificationSender.ScheduleSendNotificationAsync(notification);
        }
    }
}

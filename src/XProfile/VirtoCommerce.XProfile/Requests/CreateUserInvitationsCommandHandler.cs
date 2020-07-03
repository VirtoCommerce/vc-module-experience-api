using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class CreateUserInvitationsCommandHandler : IRequestHandler<CreateUserInvitationsCommand, IdentityResult>
    {
        private readonly IMemberServiceX _memberService;
        private readonly IServiceProvider _services;

        public CreateUserInvitationsCommandHandler(IMemberServiceX memberService, IServiceProvider services)
        {
            _memberService = memberService;
            _services = services;
        }

        public async Task<IdentityResult> Handle(CreateUserInvitationsCommand request, CancellationToken cancellationToken)
        {
            var result = IdentityResult.Success;

            var organizationId = request.OrganizationId;
            //If it is organization invitation need to check authorization for this action
            //if (!string.IsNullOrEmpty(organizationId))
            //{
            //    var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, SecurityConstants.Permissions.CanInviteUsers);
            //    if (!authorizationResult.Succeeded)
            //    {
            //        return Unauthorized();
            //    }
            //}

            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                foreach (var email in request.Emails)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    // TODO: move impl. from storefront
                    //if (user == null)
                    //{
                    //    user = new User
                    //    {
                    //        UserName = email,
                    //        StoreId = request.StoreId,
                    //        Email = email,
                    //    };
                    //    var roles = request.Roles.Select(x => new Model.Security.Role { Id = x }).ToList();
                    //    //Add default role for organization member invitation
                    //    if (roles.IsNullOrEmpty() && !string.IsNullOrEmpty(organizationId))
                    //    {
                    //        roles = new[] { SecurityConstants.Roles.OrganizationEmployee }.ToList();
                    //    }
                    //    user.Roles = roles;
                    //    result = UserActionIdentityResult.Instance(await _userManager.CreateAsync(user));
                    //}

                    //if (result.Succeeded)
                    //{
                    //    user = await _userManager.FindByNameAsync(user.UserName);
                    //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //    var callbackUrl = Url.Action("ConfirmInvitation", "Account", new { OrganizationId = organizationId, user.Email, Token = token }, Request.Scheme, host: WorkContext.CurrentStore.Host);
                    //    var inviteNotification = new RegistrationInvitationNotification(request.StoreId, request.Language)
                    //    {
                    //        InviteUrl = callbackUrl,
                    //        Sender = request.StoreEmail,
                    //        Recipient = user.Email
                    //    };
                    //    var sendingResult = await _platformNotificationApi.SendNotificationByRequestAsync(inviteNotification.ToNotificationDto());
                    //    if (sendingResult.IsSuccess != true)
                    //    {
                    //        var errors = result.Errors.Concat(new IdentityError[] { new IdentityError() { Description = sendingResult.ErrorMessage } }).ToArray();
                    //        result = IdentityResult.Failed(errors);
                    //    }
                    //}
                }
            }
            return result;
        }
    }
}

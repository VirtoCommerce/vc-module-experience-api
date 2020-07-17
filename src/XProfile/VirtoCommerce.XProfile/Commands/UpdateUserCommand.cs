using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateUserCommand : ApplicationUser, ICommand<IdentityResult>
    {
        public UpdateUserCommand()
        {

        }

        public UpdateUserCommand(string email = default, string id = default, bool isAdministrator = default, bool lockoutEnabled = default, DateTimeOffset? lockoutEnd = default, string memberId = default, string phoneNumber = default, bool phoneNumberConfirmed = default, string photoUrl = default, IList<Role> roles = default, bool twoFactorEnabled = default, string userName = default, string userType = default)
        {
            Email = email;
            Id = id;
            IsAdministrator = isAdministrator;
            LockoutEnabled = lockoutEnabled;
            LockoutEnd = lockoutEnd;
            MemberId = memberId;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            PhotoUrl = photoUrl;
            Roles = roles;
            TwoFactorEnabled = twoFactorEnabled;
            UserName = userName;
            UserType = userType;
        }
    }
}

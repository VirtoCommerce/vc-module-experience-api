using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateUserCommand : ApplicationUser, ICommand<IdentityResult>
    {
        public CreateUserCommand()
        {

        }

        public CreateUserCommand(string createdBy = default, DateTime createdDate = default, string email = default, string id = default, bool isAdministrator = default, bool lockoutEnabled = default, DateTimeOffset? lockoutEnd = default, string memberId = default, string password = default, string phoneNumber = default, bool phoneNumberConfirmed = default, string photoUrl = default, IList<Role> roles = default, string storeId = default, bool twoFactorEnabled = default, string userName = default, string userType = default)
        {
            CreatedBy = createdBy;
            CreatedDate = createdDate;
            Email = email;
            Id = id;
            IsAdministrator = isAdministrator;
            LockoutEnabled = lockoutEnabled;
            LockoutEnd = lockoutEnd;
            MemberId = memberId;
            Password = password;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            PhotoUrl = photoUrl;
            Roles = roles;
            StoreId = storeId;
            TwoFactorEnabled = twoFactorEnabled;
            UserName = userName;
            UserType = userType;
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateRoleCommand : ICommand<IdentityResult>
    {
        public Role Role { get; set; } = new Role();
        public UpdateRoleCommand(string id = default, string description = null, string name = null, IList<Permission> permissions = null)
        {
            Role.Id = id;
            Role.Description = description;
            Role.Name = name;
            Role.Permissions = permissions;
        }
    }
}

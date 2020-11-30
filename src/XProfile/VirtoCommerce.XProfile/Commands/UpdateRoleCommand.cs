using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateRoleCommand : Role, ICommand<IdentityResult>
    {
        public string UserId { get; set; }

        public UpdateRoleCommand(string id = default, string description = null, string name = null, IList<Permission> permissions = null)
        {
            Id = id;
            Description = description;
            Name = name;
            Permissions = permissions;
        }
    }
}

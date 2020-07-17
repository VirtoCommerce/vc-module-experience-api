using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class DeleteUserCommand : ICommand<IdentityResult>
    {
        public string[] UserNames { get; set; }

        public DeleteUserCommand()
        {

        }

        public DeleteUserCommand(string[] userNames)
        {
            UserNames = userNames;
        }
    }
}

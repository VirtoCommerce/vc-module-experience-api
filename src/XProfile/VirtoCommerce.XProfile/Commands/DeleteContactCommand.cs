using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class DeleteContactCommand : ICommand<bool>
    {
        public DeleteContactCommand()
        {

        }

        public DeleteContactCommand(string contactId)
        {
            ContactId = contactId;
        }

        public string ContactId { get; set; }
    }
}

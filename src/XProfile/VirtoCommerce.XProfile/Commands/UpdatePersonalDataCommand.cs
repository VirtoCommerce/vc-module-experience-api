using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates.Contact;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdatePersonalDataCommand : ICommand<IdentityResult>
    {
        public string UserId { get; set; }
        public PersonalData PersonalData { get; set; }
    }
}

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.ExternalSignIn;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public class ExternalSignInUserBuilder : IExternalSignInUserBuilder
    {
        private readonly IMemberService _memberService;

        public ExternalSignInUserBuilder(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task BuildNewUser(ApplicationUser user, ExternalLoginInfo externalLoginInfo)
        {
            if (user.MemberId is null && user.UserType == UserType.Customer.ToString())
            {
                var contact = AbstractTypeFactory<Contact>.TryCreateInstance();
                contact.Name = externalLoginInfo.Principal.FindFirstValue("name");
                contact.FirstName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName);
                contact.LastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname);
                contact.Emails = [user.Email];

                await _memberService.SaveChangesAsync([contact]);

                user.MemberId = contact.Id;
            }
        }
    }
}

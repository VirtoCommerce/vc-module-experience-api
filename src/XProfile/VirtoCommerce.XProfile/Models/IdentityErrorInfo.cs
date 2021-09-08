using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Models
{
    public class IdentityErrorInfo : IdentityError
    {
        public int? ErrorParameter { get; set; }
    }
}

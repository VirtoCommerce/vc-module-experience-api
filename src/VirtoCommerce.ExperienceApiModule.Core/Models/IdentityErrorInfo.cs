using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.ExperienceApiModule.Core.Models
{
    public class IdentityErrorInfo : IdentityError
    {
        public int? ErrorParameter { get; set; }
    }
}

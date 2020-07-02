using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class Profile
    {
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Member associated with user 
        /// </summary>
        public Contact Contact { get; set; }
    }
}

using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApi.ProfileModule.Core.Models
{
    public class Profile
    {
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Member associated with user 
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// All user permissions
        /// </summary>
        public IEnumerable<string> Permissions { get; set; }

        /// <summary>
        /// Indicates that user has no orders
        /// </summary>
        public bool IsFirstTimeBuyer { get; set; }
    }
}

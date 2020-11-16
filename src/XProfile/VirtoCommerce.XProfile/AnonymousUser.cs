using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class AnonymousUser : ApplicationUser
    {
        public static AnonymousUser Instance => new AnonymousUser();
        private AnonymousUser()
        {
            UserName = "Anonymous";
            Roles = new List<Role>();
        }
    }
}

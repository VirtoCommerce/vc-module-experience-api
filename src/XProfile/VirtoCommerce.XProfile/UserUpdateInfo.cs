using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public partial class UserUpdateInfo : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }
}

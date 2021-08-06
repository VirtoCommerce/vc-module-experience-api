using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class CheckUsernameUniquenessQuery : IQuery<CheckUsernameUniquenessResponse>
    {
        public string Username { get; set; }
    }
}

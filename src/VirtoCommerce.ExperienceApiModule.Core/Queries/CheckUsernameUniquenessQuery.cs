using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class CheckUsernameUniquenessQuery : IQuery<CheckUsernameUniquenessResponse>
    {
        public string Username { get; set; }
    }
}

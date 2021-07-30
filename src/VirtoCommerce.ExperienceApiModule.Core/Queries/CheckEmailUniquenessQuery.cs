using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class CheckEmailUniquenessQuery : IQuery<CheckEmailUniquenessResponse>
    {
        public string Email { get; set; }
    }
}

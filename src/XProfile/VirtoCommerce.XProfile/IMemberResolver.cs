using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    //PT-5379: Move into ContactModule
    public interface IMemberResolver
    {
        Task<Member> ResolveMemberByIdAsync(string userId);
    }
}

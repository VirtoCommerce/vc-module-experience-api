using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public interface IUserManagerCore
    {
        Task<bool> IsLockedOutAsync(ApplicationUser user);
        Task CheckUserState(string userId);
    }
}

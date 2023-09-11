using System;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public interface IUserManagerCore
    {
        Task<bool> IsLockedOutAsync(ApplicationUser user);

        [Obsolete("Use CheckUserState(string userId, bool allowAnonymous)", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task CheckUserState(string userId);

        Task CheckUserState(string userId, bool allowAnonymous);
    }
}

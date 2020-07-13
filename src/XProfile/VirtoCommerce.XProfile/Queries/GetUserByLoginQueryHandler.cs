using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserByLoginQueryHandler : IQueryHandler<GetUserByLoginQuery, ApplicationUser>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public GetUserByLoginQueryHandler(Func<UserManager<ApplicationUser>> userManager)
        {
            _userManagerFactory = userManager;
        }

        public async Task<ApplicationUser> Handle(GetUserByLoginQuery request, CancellationToken cancellationToken)
        {
            using (var userManager = _userManagerFactory())
            {
                return await userManager.FindByLoginAsync(request.LoginProvider, request.ProviderKey);
            }
        }
    }
}

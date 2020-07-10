using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserByNameQueryHandler : IQueryHandler<GetUserByNameQuery, ApplicationUser>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public GetUserByNameQueryHandler(Func<UserManager<ApplicationUser>> userManager)
        {
            _userManagerFactory = userManager;
        }

        public async Task<ApplicationUser> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
        {
            using (var userManager = _userManagerFactory())
            {
                return await userManager.FindByNameAsync(request.UserName);
            }
        }
    }
}

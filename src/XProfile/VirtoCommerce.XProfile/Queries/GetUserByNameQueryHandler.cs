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
        private readonly IServiceProvider _services;

        public GetUserByNameQueryHandler(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<ApplicationUser> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await _userManager.FindByNameAsync(request.UserName);
            }
        }
    }
}

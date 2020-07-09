using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, ApplicationUser>
    {
        private readonly IServiceProvider _services;

        public GetUserByEmailQueryHandler(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<ApplicationUser> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await _userManager.FindByEmailAsync(request.Email);
            }
        }
    }
}
